namespace VivantioApiInteractive.Dtos;

internal record TicketAddNoteDto
{
    public required List<int> AffectedTickets { get; init; }
    public required string Notes { get; init; }
    public int Effort { get; init; } = 0;
    public List<EmbeddedAttachmentDto> Attachments { get; init; } = [];
}