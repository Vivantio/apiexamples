using Spectre.Console;
using System.Xml.Linq;

namespace VivantioApiInteractive
{
    public class ClientBaseDto
    {
        public required string Name { get; set; }
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


    public class Client
    {
        public static async Task UpdateClient()
        {
            var client = new ClientUpdateDto
            {
                Id = 74, // Assuming you want to update the client with ID 1
                Reference = "freda",
                Name = "Quantum Enterprises",
                Notes = "Some Text",
                Alert = "This is an updated alert text new new new.",
                ExternalKey = "ext-quantumenterprises-ie-1",
                ExternalSource = "vivantio-qa-manager",
            };

            await Helper.UpdateItemAsync(client, "Client/Update");

            AnsiConsole.MarkupLine($"Record was updated");
        }

        public static async Task InsertClient()
        {
            var indentifier = $"vae-{Guid.NewGuid()}";

            var client = new ClientInsertDto
            {
                Reference = indentifier,
                Name = indentifier,
                Notes = "Some Text",
                Alert = "This is an updated alert text new new new.",
                ExternalKey = indentifier,
                ExternalSource = "vivantio-api-examples",
                StatusId = 65,
                RecordTypeId = 6,
            };

            await Helper.UpdateItemAsync(client, "Client/Insert");

            AnsiConsole.MarkupLine($"Record {indentifier} was inserted");
        }


        public static async Task ShowMenu()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select a Client operation[/]")
                        .PageSize(5)
                        .AddChoices("Insert", "Update", "Back"));

                switch (choice)
                {
                    case "Insert":
                        await InsertClient();
                        Helper.Pause();
                        break;
                    case "Update":
                        await UpdateClient();
                        Helper.Pause();
                        break;
                    case "Back":
                        backToMain = true;
                        break;
                }
            }
        }
    }
}
