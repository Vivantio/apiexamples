namespace VivantioApiInteractive.Dtos;

internal record EmbeddedAttachmentDto
{
    public required string Name { get; init; }
    public required byte[] Content { get; init; }
    public string Description { get; init; } = "No description supplied";
    public bool MarkPrivate { get; init; } = false;
    public AttachmentType AttachmentType { get; init; } = AttachmentType.File;
}
