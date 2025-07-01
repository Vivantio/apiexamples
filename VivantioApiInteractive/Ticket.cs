using Spectre.Console;
using System.Text.Json.Serialization;

namespace VivantioApiInteractive
{

    public class TicketBaseDto
    {
        public int Id { get;}
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public string? CallerName { get; set; }
        public int TakenById { get; set; }
        public int OwnerId { get; set; }
        public int ThirdPartyId { get; set; }
        public string? ThirdPartyRef { get; set; }
        public DateTime ThirdPartyLogDate { get; set; }
        public int PriorityId { get; set; }
        public StatusType? StatusType { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? Solution { get; set; }
        public int LocationId { get; set; }
        public int LastActionId { get; set; }
        public string? CallerPhone { get; set; }
        public string? CallerEmail { get; set; }
        public int TotalEffort { get; set; }
        public int LastModifiedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public int StatusId { get; set; }
        public int ImpactId { get; set; }
        public int CallerId { get; set; }
        public string? PriorityName { get; set; }
        public string? TakenByName { get; set; }
        public string? GroupName { get; set; }
        public string? OwnerName { get; set; }
        public string? ClientName { get; set; }
        public string? CategoryLineage { get; set; }
        public string? StatusName { get; set; }
        public int WorkflowId { get; set; }
        public string? DisplayId { get; set; }
        public bool Resolved { get; set; }
        public bool IsDeferred { get; set; }
        public int AssigneeType { get; set; }
        public int CallOrigin { get; set; }
        public int AssigneeClientId { get; set; }
        public int AssigneeCallerId { get; set; }
        public string? AssigneeExpression { get; set; }
        public string? AssignedToEmail { get; set; }
        public int IntelligentAssignmentGroup { get; set; }
        public bool IntelligentAssignmentExcludeOutOfOffice { get; set; }
        public string? AssignedToGroupName { get; set; }
        public string? AssignedToIndividualName { get; set; }
        public DateTime ResolvedDate { get; set; }
        public string? AssignedToSummary { get; set; }
        public string? CCAddressList { get; set; }
        public string? DescriptionHtml { get; set; }
        public string? SolutionHtml { get; set; }
        public bool Deleted { get; set; }
        public int NextStepId { get; set; }
        public string? RecordTypeNameSingular { get; set; }
    }

    public class TicketInsertDto : TicketBaseDto
    {
        public required int RecordTypeId { get; set; }
        public required string Title { get; set; }
    }

    public class TicketTypeDto
    {
        public int Id { get; }
        public string? NamePlural { get; }
        public string? NameSingular { get; }
        public bool Enabled { get; }
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
            var title = AnsiConsole.Prompt(
                new TextPrompt<string>("Please supply a Title for this Ticket:")
                .PromptStyle("blue")
                .AllowEmpty() // Useed to force validation
                    .Validate(name =>
                    {
                        return string.IsNullOrWhiteSpace(name)
                            ? ValidationResult.Error("[red]Title cannot be empty[/]")
                            : ValidationResult.Success();
                    }));


            // Choose a client for the ticket
            var clients = await Client.GetClients();
            var selectedClient = Client.SelectClient(clients);

            if (selectedClient == null)
            {
                AnsiConsole.MarkupLine("[red]Error: No client was selected. Cannot proceed with ticket creation.[/]");
                Spectre.EnterToContinue();
                return;
            }

            var ticket = new TicketInsertDto
            {
                RecordTypeId = 11, // Incidents
                ClientId = selectedClient.Id,
                CallerId = 1,
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
