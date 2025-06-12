using Spectre.Console;
using System.Text.Json.Serialization;

namespace VivantioApiInteractive
{

    public class TicketBaseDto
    {
        public int Id { get; set; }
        public string? DisplayId { get; set; }
        public string? Description { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusType? StatusType { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryLineage { get; set; }
    }

    public class TicketInsertDto : TicketBaseDto
    {
        public required int RecordTypeId { get; set; }
        public required string Title { get; set; }
    }

    public class TicketTypeDto
    {
        public int Id { get; set; }
        public string? NamePlural { get; set; }
        public string? NameSingular { get; set; }
        public bool Enabled { get; set; }
    }


    public static class Ticket
    {
        public static async Task ShowTicketTypes()
        {
            var response = await ApiUtility.SendRequestAsync<SelectResponse<TicketTypeDto>>("Configuration/TicketTypeSelectAll");
            var ticketTypes = response?.Results ?? [];

            var ticketTypesNames = ticketTypes
                .Where(t => !string.IsNullOrEmpty(t.NameSingular))
                .Select(t => $"{t.NameSingular} ({t.Id})")
                .ToList();

            AnsiConsole.MarkupLine($"These are the Ticket Types configured in this system (RecordTypeId is in parentheses):");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("- " + string.Join("\n- ", ticketTypesNames));
            AnsiConsole.WriteLine();
            Spectre.EnterToContinue();
        }

        public static async Task InsertTicket()
        {
            var random = RandomProvider.Instance;
            var title = $"Ticket-{Helper.GenerateRandomString(5, random)}-{DateTime.Now:yyyyMMddHHmmss}";

            var ticket = new TicketInsertDto
            {
                RecordTypeId = 11, // Incidents
                Title = title,
                Description = Helper.GetLoremIpsum(),
            };

            int insertedTicketId;

            var response = await ApiUtility.SendRequestAsync<InsertResponse, TicketInsertDto>("Ticket/Insert", ticket);

            if (response != null && response.Successful)
            {
                insertedTicketId = response?.InsertedItemId ?? 0;

                AnsiConsole.MarkupLine($"Ticket [blue]{title}[/] was inserted. Adding Attachments...");

                // Add PDF and text attachments to the newly created client
                await AddTicketAttachments(insertedTicketId, title);

                AnsiConsole.MarkupLine($"Attachments for [blue]{title}[/] were added.");

                Spectre.EnterToContinue();
            }
            else
            {
                var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                AnsiConsole.MarkupLine($"[red]Error inserting Ticket: {errorMessage}[/]");
                Spectre.EnterToContinue();
            }
        }

        private static async Task AddTicketAttachments(int ticketId, string title)
        {
            var identifierText = "a ticket";
            var fileContentText = $"This attachment was created for Ticket {title}";

            var tasks = new List<Task>
                {
                    Attachment.InsertAttachment((int)SystemAreaId.Ticket, ticketId, AttachmentFileType.PDF, identifierText, fileContentText, 2),
                    Attachment.InsertAttachment((int)SystemAreaId.Ticket, ticketId, AttachmentFileType.Text, identifierText, fileContentText, 2)
                };

            await Task.WhenAll(tasks);
        }

        public static async Task ShowMenu()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title("Select a Ticket operation")
                        .PageSize(5)
                        .AddChoices("Show Ticket Types", Spectre.SubMenuInsert, Spectre.SubMenuUpdate, Spectre.SubMenuBack));

                switch (choice)
                {
                    case "Show Ticket Types":
                        await ShowTicketTypes();
                        break;
                    case Spectre.SubMenuInsert:
                        await InsertTicket();
                        break;
                    case Spectre.SubMenuUpdate:
                        await InsertTicket();
                        break;
                    case Spectre.SubMenuBack:
                        backToMain = true;
                        break;
                }
            }
        }
    }
}
