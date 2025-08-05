namespace VivantioApiInteractive
{
    public record ClientBaseDto
    {
        public string? Name { get; init; }
        public string? WebSite { get; init; }
        public string? Email { get; init; }
        public string? Alert { get; init; }
        public string? ExternalKey { get; init; }
        public string? ExternalSource { get; init; }
        public string? Notes { get; init; }
        public string? EmailSuffix { get; init; }
    }

    public record ClientUpdateDto : ClientBaseDto
    {
        public required int Id { get; init; }
        public required string Reference { get; init; }
    }

    public record ClientInsertDto : ClientBaseDto
    {
        public required string Reference { get; init; }
        public required int RecordTypeId { get; init; }
        public required int StatusId { get; init; }
    }

    public record ClientSelectDto : ClientBaseDto
    {
        public int Id { get; init; }
        public string? Reference { get; init; }
        public DateTime? CreatedDate { get; init; }
        public DateTime? UpdateDate { get; init; }
        public bool? Deleted { get; init; }
        public string? StatusName { get; init; }
    }

    public class Client
    {
        public static async Task InsertClient()
        {
            var random = RandomProvider.Instance;

            var companyName = Helper.GetRandomCompanyName(random);
            var reference = $"{companyName.Replace(" ", "")}-{Helper.GenerateRandomString(2, random)}";
            //var reference = "intentional duplicate for testing";
            var domain = $"{companyName.ToLower().Replace(" ", "").Replace(".", "")}{Helper.GetRandomTopLevelDomain(random)}";

            var client = new ClientInsertDto
            {
                Reference = reference,
                Name = companyName,
                WebSite = $"https://www.{domain}",
                Email = $"info@{domain}",
                Notes = Helper.GetLoremIpsum(),
                Alert = "This is an updated alert text new new new.",
                ExternalKey = $"ext-{reference}",
                ExternalSource = Helper.ExternalSource,
                StatusId = 65,
                RecordTypeId = 6,
            };

            int insertedClientId;

            var response = await ApiUtility.SendRequestAsync<InsertResponse, ClientInsertDto>("Client/Insert", client);

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
                    await Asset.InsertAssetReleation(insertedAssetIds.ToList(), insertedClientId, SystemAreaId.Client);
                    await Asset.InsertAssetReleation(insertedAssetIds.ToList(), locationId, SystemAreaId.Location);
                }

                AnsiConsole.MarkupLine($"Assets for [blue]{reference}[/] were added. Adding Callers...");

                foreach (var locationId in locationIds)
                {
                    await Caller.InsertCallers(insertedClientId, domain, locationId);
                }

                AnsiConsole.MarkupLine($"Callers for [blue]{reference}[/] were added. That's it!");
                Spectre.EnterToContinue();
            }
            else
            {
                var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                AnsiConsole.MarkupLine($"[red]Error inserting Client: {errorMessage}[/]");
                Spectre.EnterToContinue();
            }
        }

        private static async Task AddClientAttachments(int clientId, string reference)
        {
            var identifierText = "a client";
            var fileContentText = $"This attachment was created for Client {reference}";

            var tasks = new List<Task>
                {
                    Attachment.InsertAttachment((int)SystemAreaId.Client, clientId, AttachmentFileType.PDF, identifierText, fileContentText, 2),
                    Attachment.InsertAttachment((int)SystemAreaId.Client, clientId, AttachmentFileType.Text, identifierText, fileContentText, 2)
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

                await ApiUtility.SendRequestAsync<BaseResponse, ClientUpdateDto>("Client/Update", clientToUpdate);
                AnsiConsole.MarkupLine($"Client [blue]{selectedClient.Name}[/] was updated.");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Selected client is null.[/]");
            }
            Spectre.EnterToContinue();
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
                        .AddChoices(Spectre.SubMenuInsert, Spectre.SubMenuUpdate, Spectre.SubMenuBack));

                switch (choice)
                {
                    case Spectre.SubMenuInsert:
                        await InsertClient();
                        break;
                    case Spectre.SubMenuUpdate:
                        await UpdateClient();
                        break;
                    case Spectre.SubMenuBack:
                        backToMain = true;
                        break;
                }
            }
        }

        public static async Task<List<ClientSelectDto>> GetClients()
        {
            // Define a query to select clients based on the ExternalSource
            var query = new Query();
            query.Items.Add(new QueryItem
            {
                FieldName = "ExternalSource",
                Op = Operator.Equals,
                Value = Helper.ExternalSource
            });
            query.Items.Add(new QueryItem
            {
                FieldName = "Deleted",
                Op = Operator.DoesNotEqual,
                Value = (int)StatusType.Deleted
            });

            var response = await ApiUtility.SendRequestAsync<SelectResponse<ClientSelectDto>, SelectRequest>("Client/Select", new SelectRequest { Query = query });
            return response?.Results ?? [];
        }

        public static ClientSelectDto? SelectClient(List<ClientSelectDto> clients)
        {
            var clientNames = clients
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name!)
                .ToList();

            // If no clients found, display a message and return null
            if (clientNames.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No Clients found with the specified criteria.[/]");
                Spectre.EnterToContinue();
                return null; // Return null instead of using 'return;' to satisfy the return type
            }

            // Display a list of client names for selection
            var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select a Client:").PageSize(20).AddChoices(clientNames));

            return clients.FirstOrDefault(c => c.Name == selected);
        }
    }
}
