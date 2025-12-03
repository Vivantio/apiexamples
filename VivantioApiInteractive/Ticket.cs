namespace VivantioApiInteractive;

internal static class Ticket
{
    public static async Task ShowTicketTypes()
    {
        var response = await ApiHelper.SendRequestAsync<SelectResponse<TicketTypeDto>>("Configuration/TicketTypeSelectAll");
        var ticketTypes = response?.Results ?? [];

        var ticketTypesNames = ticketTypes
            .Where(t => !string.IsNullOrEmpty(t.NameSingular))
            .Select(t => $"{t.NameSingular} ({t.Id})")
            .ToList();

        AnsiConsole.MarkupLine($"These are the Ticket Types configured in this system (RecordTypeId is in parentheses):");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("- " + string.Join("\n- ", ticketTypesNames));
        AnsiConsole.WriteLine();
        SpectreHelper.EnterToContinue();
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
            SpectreHelper.EnterToContinue();
            return;
        }

        var ticket = new TicketInsertDto
        {
            RecordTypeId = (int)RecordType.Incidents,
            ClientId = selectedClient.Id,
            CallerId = 1,
            Title = title,
            Description = RandomStringHelper.GetLoremIpsum(),
        };

        int insertedTicketId;

        var response = await ApiHelper.SendRequestAsync<InsertResponse, TicketInsertDto>("Ticket/Insert", ticket);

        if (response != null && response.Successful)
        {
            insertedTicketId = response?.InsertedItemId ?? 0;

            AnsiConsole.MarkupLine($"Ticket [blue]{ticket.Title}[/] was inserted. Adding Attachments...");

            // Add PDF and text attachments to the newly created client
            await AddTicketAttachments(ticket.Id, ticket.Title);

            AnsiConsole.MarkupLine($"Attachments for [blue]{ticket.Title}[/] were added. Adding a Note...");

            await AddTicketNote((insertedTicketId, ticket.Title), "Note added on Insert of Ticket");

            SpectreHelper.EnterToContinue();
        }
        else
        {
            var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
            AnsiConsole.MarkupLine($"[red]Error inserting Ticket: {errorMessage}[/]");
            SpectreHelper.EnterToContinue();
        }
    }

    public static async Task UpdateTicket()
    {

        // Prompt for notes text so there is something to update
        var notesText = AnsiConsole.Prompt(
            new TextPrompt<string>("Please supply some Note text for this Ticket:")
            .PromptStyle("blue")
            .AllowEmpty() // Useed to force validation
                .Validate(name =>
                {
                    return string.IsNullOrWhiteSpace(name)
                        ? ValidationResult.Error("[red]Note text cannot be empty[/]")
                        : ValidationResult.Success();
                }));

        // Extract a list of client names for selection
        var tickets = await GetTickets();

        var selectedTicket = SelectTicket(tickets);

        if (selectedTicket != null)
        {
            await AddTicketNote((selectedTicket.Id, selectedTicket.Title), notesText);
            AnsiConsole.MarkupLine($"Ticket [blue]{selectedTicket.Title}[/] was updated with a Note.");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error: Selected ticket is null.[/]");
        }
        SpectreHelper.EnterToContinue();
    }

    private static async Task AddTicketAttachments(int ticketId, string title)
    {
        var identifierText = "a ticket";
        var fileContentText = $"This attachment was created for Ticket {title}";

        var tasks = new List<Task>
            {
                Attachment.InsertAttachment(SystemArea.Ticket, ticketId, AttachmentFileType.PDF, identifierText, fileContentText, 2),
                Attachment.InsertAttachment(SystemArea.Ticket, ticketId, AttachmentFileType.Text, identifierText, fileContentText, 2)
            };

        await Task.WhenAll(tasks);
    }

    private static async Task AddTicketNote((int Id, string Title) ticketInfo, string notes)
    {
        var pdfAttachmentDescription = $"This is a sample Note PDF attachment for Ticket {ticketInfo.Title} created on {DateTime.Now}";
        var txtAttachmentDescription = $"This is a sample Note text attachment for Ticket {ticketInfo.Title} created on {DateTime.Now}";

        var ticketNoteToAdd = new TicketAddNoteDto
        {
            AffectedTickets = new List<int> { ticketInfo.Id },
            Notes = notes,
            Effort = 10,
            Attachments = new List<EmbeddedAttachmentDto>
                {
                    new EmbeddedAttachmentDto
                    {
                        Name = $"This is a PDF document for a ticket note for Ticket {ticketInfo.Title}.pdf",
                        AttachmentType = AttachmentType.File,
                        Description = pdfAttachmentDescription,
                        Content = Attachment.CreatePdfFile(pdfAttachmentDescription),
                        MarkPrivate = true
                    },
                    new EmbeddedAttachmentDto
                    {
                        Name = $"This is a text document for a ticket note for Ticket {ticketInfo.Title}.txt",
                        AttachmentType = AttachmentType.File,
                        Description = txtAttachmentDescription,
                        Content = Attachment.CreateTextFile(txtAttachmentDescription)
                    }
                }
        };

        await ApiHelper.SendRequestAsync<BaseResponse, TicketAddNoteDto>("Ticket/AddNote", ticketNoteToAdd);
    }

    internal static async Task<List<TicketSelectDto>> GetTickets()
    {
        // Define a query to select tickets based on the ExternalSource
        var query = new Query();
        query.Items.Add(new QueryItem
        {
            FieldName = "StatusType",
            Op = Operator.Equals,
            Value = (int)StatusType.Open
        });
        query.Items.Add(new QueryItem
        {
            FieldName = "Deleted",
            Op = Operator.DoesNotEqual,
            Value = (int)StatusType.Deleted
        });

        var response = await ApiHelper.SendRequestAsync<SelectResponse<TicketSelectDto>, SelectRequest>("Ticket/Select", new SelectRequest { Query = query });
        return response?.Results ?? [];
    }

    internal static TicketSelectDto? SelectTicket(List<TicketSelectDto> tickets)
    {
        var ticketTitles = tickets
            .Where(t => !string.IsNullOrEmpty(t.Title))
            .Select(t => t.Title!)
            .ToList();

        // If no tickets found, display a message and return null
        if (ticketTitles.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Tickets found with the specified criteria.[/]");
            SpectreHelper.EnterToContinue();
            return null; // Return null instead of using 'return;' to satisfy the return type
        }

        // Display a list of client names for selection
        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select a Ticket:").PageSize(20).AddChoices(ticketTitles));

        return tickets.FirstOrDefault(t => t.Title == selected);
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
                    .AddChoices("Show Ticket Types", SpectreHelper.SubMenuInsert, SpectreHelper.SubMenuUpdate, SpectreHelper.SubMenuBack));

            switch (choice)
            {
                case "Show Ticket Types":
                    await ShowTicketTypes();
                    break;
                case SpectreHelper.SubMenuInsert:
                    await InsertTicket();
                    break;
                case SpectreHelper.SubMenuUpdate:
                    await UpdateTicket();
                    break;
                case SpectreHelper.SubMenuBack:
                    backToMain = true;
                    break;
            }
        }
    }
}
