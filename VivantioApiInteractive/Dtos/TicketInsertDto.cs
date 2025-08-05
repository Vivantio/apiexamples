namespace VivantioApiInteractive.Dtos;

internal record TicketInsertDto : TicketBaseDto
{
    public required int RecordTypeId { get; init; }
    public required string Title { get; init; }
}
