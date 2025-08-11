namespace VivantioApiInteractive.Dtos;

internal record TicketSelectDto
{
    // This DTO is used for selecting a ticket from a list, containing only the necessary fields for selection.
    public int Id { get; init; }
    public required string Title { get; init; }
    public StatusType? StatusType { get; init; }
}
