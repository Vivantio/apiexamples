namespace VivantioApiInteractive.Dtos;

internal record LocationDto
{
    public required string Name { get; init; }
    public required int ClientId { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Address3 { get; init; }
    public string? City { get; init; }
    public string? County { get; init; }
    public string? PostCode { get; init; }
    public string? Country { get; init; }
    public string? Phone { get; init; }
    public string? Notes { get; init; }
    public string? ExternalKey { get; init; }
    public string? ExternalSource { get; init; }
}
