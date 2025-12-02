namespace VivantioApiInteractive;

internal class Client
{
    public static async Task InsertClient()
    {
        var random = RandomProvider.Instance;

        var companyName = RandomStringHelper.GetRandomCompanyName(random);
        var reference = $"{companyName.Replace(" ", "")}-{RandomStringHelper.GenerateRandomString(2, random)}";
        //var reference = "intentional duplicate for testing";
        var domain = $"{companyName.ToLower().Replace(" ", "").Replace(".", "")}{RandomStringHelper.GetRandomTopLevelDomain(random)}";

        var client = new ClientInsertDto
        {
            Reference = reference,
            Name = companyName,
            WebSite = $"https://www.{domain}",
            Email = $"info@{domain}",
            Notes = RandomStringHelper.GetLoremIpsum(),
            Alert = "This is an updated alert text new new new.",
            ExternalKey = $"ext-{reference}",
            ExternalSource = AppHelper.ExternalSource,
            StatusId = 65,
            RecordTypeId = 6,
        };

        int insertedClientId;

        var response = await ApiHelper.SendRequestAsync<InsertResponse, ClientInsertDto>("Client/Insert", client);

        if (response != null && response.Successful)
        {
            insertedClientId = response?.InsertedItemId ?? 0;

            AnsiConsole.MarkupLine($"Client [blue]{reference}[/] was inserted. Adding Attachments...");

            // Add PDF and text attachments to the newly created client
            await AddClientAttachments(insertedClientId, reference);

            AnsiConsole.MarkupLine($"Attachments for [blue]{reference}[/] were added. Adding Locations...");

            var locationIds = await Location.InsertLocations(insertedClientId);

            AnsiConsole.MarkupLine($"Locations for [blue]{reference}[/] were added. Adding Assets...");

            // Clients can have Assets, so we insert corporate assets and link them to the Client and Locations
            foreach (var locationId in locationIds)
            {
                var insertedAssetIds = await Asset.InsertCorporateAssets();
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), insertedClientId, SystemArea.Client);
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), locationId, SystemArea.Location);
            }

            AnsiConsole.MarkupLine($"Assets for [blue]{reference}[/] were added. Adding Callers...");

            foreach (var locationId in locationIds)
            {
                await Caller.InsertCallers(insertedClientId, domain, locationId);
            }

            AnsiConsole.MarkupLine($"Callers for [blue]{reference}[/] were added. That's it!");
            SpectreHelper.EnterToContinue();
        }
        else
        {
            var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
            AnsiConsole.MarkupLine($"[red]Error inserting Client: {errorMessage}[/]");
            SpectreHelper.EnterToContinue();
        }
    }

    private static async Task AddClientAttachments(int clientId, string reference)
    {
        var identifierText = "a client";
        var fileContentText = $"This attachment was created for Client {reference}";

        var tasks = new List<Task>
            {
                Attachment.InsertAttachment((int)SystemArea.Client, clientId, AttachmentFileType.PDF, identifierText, fileContentText, 2),
                Attachment.InsertAttachment((int)SystemArea.Client, clientId, AttachmentFileType.Text, identifierText, fileContentText, 2)
            };

        await Task.WhenAll(tasks);
    }

    public static async Task UpdateClient()
    {

        // Prompt for notes text so there is something to update
        var notesText = AnsiConsole.Prompt(
            new TextPrompt<string>("Please supply some Notes text for this Client:")
            .PromptStyle("blue")
            .AllowEmpty() // Useed to force validation
                .Validate(name =>
                {
                    return string.IsNullOrWhiteSpace(name)
                        ? ValidationResult.Error("[red]Notes cannot be empty[/]")
                        : ValidationResult.Success();
                }));

        // Extract a list of client names for selection
        var clients = await GetClients();

        var selectedClient = SelectClient(clients);


        // When performing an update it is critical that all fields with exiisting values are included in the request otherwise they will be overwritten with blank values.
        if (selectedClient != null) // Ensure selectedClient is not null
        {
            var clientToUpdate = new ClientUpdateDto
            {
                Id = selectedClient.Id,
                Reference = selectedClient.Reference ?? string.Empty,
                Name = selectedClient.Name,
                Notes = notesText,
                Alert = selectedClient.Alert,
                ExternalKey = selectedClient.ExternalKey,
                ExternalSource = selectedClient.ExternalSource
            };

            await ApiHelper.SendRequestAsync<BaseResponse, ClientUpdateDto>("Client/Update", clientToUpdate);
            AnsiConsole.MarkupLine($"Client [blue]{selectedClient.Name}[/] was updated.");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error: Selected client is null.[/]");
        }
        SpectreHelper.EnterToContinue();
    }

    public static async Task ShowMenu()
    {
        bool backToMain = false;

        while (!backToMain)
        {
            AnsiConsole.Clear();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Select a Client operation")
                    .PageSize(5)
                    .AddChoices(SpectreHelper.SubMenuInsert, SpectreHelper.SubMenuUpdate, SpectreHelper.SubMenuBack));

            switch (choice)
            {
                case SpectreHelper.SubMenuInsert:
                    await InsertClient();
                    break;
                case SpectreHelper.SubMenuUpdate:
                    await UpdateClient();
                    break;
                case SpectreHelper.SubMenuBack:
                    backToMain = true;
                    break;
            }
        }
    }

    internal static async Task<List<ClientSelectDto>> GetClients()
    {
        // Define a query to select clients based on the ExternalSource
        var query = new Query();
        query.Items.Add(new QueryItem
        {
            FieldName = "ExternalSource",
            Op = Operator.Equals,
            Value = AppHelper.ExternalSource
        });
        query.Items.Add(new QueryItem
        {
            FieldName = "Deleted",
            Op = Operator.DoesNotEqual,
            Value = (int)StatusType.Deleted
        });

        var response = await ApiHelper.SendRequestAsync<SelectResponse<ClientSelectDto>, SelectRequest>("Client/Select", new SelectRequest { Query = query });
        return response?.Results ?? [];
    }

    internal static ClientSelectDto? SelectClient(List<ClientSelectDto> clients)
    {
        var clientNames = clients
            .Where(c => !string.IsNullOrEmpty(c.Name))
            .Select(c => c.Name!)
            .ToList();

        // If no clients found, display a message and return null
        if (clientNames.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Clients found with the specified criteria.[/]");
            SpectreHelper.EnterToContinue();
            return null; // Return null instead of using 'return;' to satisfy the return type
        }

        // Display a list of client names for selection
        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select a Client:").PageSize(20).AddChoices(clientNames));

        return clients.FirstOrDefault(c => c.Name == selected);
    }
}
