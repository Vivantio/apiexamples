using Spectre.Console;

namespace VivantioApiInteractive
{
    public class ClientBaseDto
    {
        public string? Name { get; set; }
        public string? WebSite { get; set; }
        public string? Email { get; set; }
        public string? Alert { get; set; }
        public string? ExternalKey { get; set; }
        public string? ExternalSource { get; set; }
        public string? Notes { get; set; }
        public string? EmailSuffix { get; set; }
    }

    public class ClientUpdateDto : ClientBaseDto
    {
        public required int Id { get; set; }
        public required string Reference { get; set; }
    }

    public class ClientInsertDto : ClientBaseDto
    {
        public required string Reference { get; set; }
        public required int RecordTypeId { get; set; }
        public required int StatusId { get; set; }
    }

    public class ClientSelectDto : ClientBaseDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Deleted { get; set; }
        public string? StatusName { get; set; }
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
            var clients = response?.Results ?? [];

            // Extract a list of client names for selection
            var clientNames = clients
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name!)
                .ToList();

            // If no clients found, display a message and return
            if (clientNames.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No Clients found with the specified criteria.[/]");
                Spectre.EnterToContinue();
                return;
            }

            // Display a list of client names for selection
            var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select a Client:").PageSize(20).AddChoices(clientNames));

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

            var selectedClient = clients.FirstOrDefault(c => c.Name == selected);

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
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Selected client is null.[/]");
            }

            AnsiConsole.MarkupLine($"Client [blue]{selected}[/] was updated.");
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
    }
}
