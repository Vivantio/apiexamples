namespace VivantioApiInteractive;

public record TicketBaseDto
{
    public int Id { get; init; }
    public DateTime OpenDate { get; init; }
    public DateTime CloseDate { get; init; }
    public string? CallerName { get; init; }
    public int TakenById { get; init; }
    public int OwnerId { get; init; }
    public int ThirdPartyId { get; init; }
    public string? ThirdPartyRef { get; init; }
    public DateTime ThirdPartyLogDate { get; init; }
    public int PriorityId { get; init; }
    public StatusType? StatusType { get; init; }
    public string? Description { get; init; }
    public int CategoryId { get; init; }
    public string? Solution { get; init; }
    public int LocationId { get; init; }
    public int LastActionId { get; init; }
    public string? CallerPhone { get; init; }
    public string? CallerEmail { get; init; }
    public int TotalEffort { get; init; }
    public int LastModifiedById { get; init; }
    public DateTime LastModifiedDate { get; init; }
    public int GroupId { get; init; }
    public int ClientId { get; init; }
    public int StatusId { get; init; }
    public int ImpactId { get; init; }
    public int CallerId { get; init; }
    public string? PriorityName { get; init; }
    public string? TakenByName { get; init; }
    public string? GroupName { get; init; }
    public string? OwnerName { get; init; }
    public string? ClientName { get; init; }
    public string? CategoryLineage { get; init; }
    public string? StatusName { get; init; }
    public int WorkflowId { get; init; }
    public string? DisplayId { get; init; }
    public bool Resolved { get; init; }
    public bool IsDeferred { get; init; }
    public int AssigneeType { get; init; }
    public int CallOrigin { get; init; }
    public int AssigneeClientId { get; init; }
    public int AssigneeCallerId { get; init; }
    public string? AssigneeExpression { get; init; }
    public string? AssignedToEmail { get; init; }
    public int IntelligentAssignmentGroup { get; init; }
    public bool IntelligentAssignmentExcludeOutOfOffice { get; init; }
    public string? AssignedToGroupName { get; init; }
    public string? AssignedToIndividualName { get; init; }
    public DateTime ResolvedDate { get; init; }
    public string? AssignedToSummary { get; init; }
    public string? CCAddressList { get; init; }
    public string? DescriptionHtml { get; init; }
    public string? SolutionHtml { get; init; }
    public bool Deleted { get; init; }
    public int NextStepId { get; init; }
    public string? RecordTypeNameSingular { get; init; }
}


public record TicketInsertDto : TicketBaseDto
{
    public required int RecordTypeId { get; init; }
    public required string Title { get; init; }
}

public record TicketTypeDto
{
    public int Id { get; init; }
    public string? NamePlural { get; init; }
    public string? NameSingular { get; init; }
    public bool Enabled { get; init; }
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
