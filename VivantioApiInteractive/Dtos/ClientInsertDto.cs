namespace VivantioApiInteractive.Dtos;

internal record ClientInsertDto : ClientBaseDto
{
    public required string Reference { get; init; }
    public required int RecordTypeId { get; init; }
    public required int StatusId { get; init; }
}
