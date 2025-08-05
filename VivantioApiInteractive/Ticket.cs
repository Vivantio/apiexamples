namespace VivantioApiInteractive;

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
