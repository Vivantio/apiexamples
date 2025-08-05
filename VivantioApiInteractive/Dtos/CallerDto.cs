namespace VivantioApiInteractive.Dtos;

internal record CallerDto
{
    public required string Name { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? DomainLoginName { get; init; }
    public required int ClientId { get; init; }
    public int? LocationId { get; init; }
    public bool SelfServiceLoginEnabled { get; init; }
    public string? Notes { get; init; }
    public string? ExternalKey { get; init; }
    public string? ExternalSource { get; init; }
    public required int RecordTypeId { get; init; }
}
