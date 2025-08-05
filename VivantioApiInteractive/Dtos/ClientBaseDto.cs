namespace VivantioApiInteractive.Dtos;

internal record ClientBaseDto
{
    public string? Name { get; init; }
    public string? WebSite { get; init; }
    public string? Email { get; init; }
    public string? Alert { get; init; }
    public string? ExternalKey { get; init; }
    public string? ExternalSource { get; init; }
    public string? Notes { get; init; }
    public string? EmailSuffix { get; init; }
}
