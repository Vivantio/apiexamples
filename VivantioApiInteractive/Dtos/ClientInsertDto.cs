namespace VivantioApiInteractive.Dtos;

public record ClientInsertDto : ClientBaseDto
{
    public required string Reference { get; init; }
    public required int RecordTypeId { get; init; }
    public required int StatusId { get; init; }
}
