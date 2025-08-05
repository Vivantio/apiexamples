namespace VivantioApiInteractive.Dtos;

public record AttachmentDto
{
    public required int SystemArea { get; init; }
    public required int ParentId { get; init; }
    public required string FileName { get; init; }
    public string? Description { get; init; }
    public required byte[] Content { get; init; }
    public bool? IsPrivate { get; init; }
}
