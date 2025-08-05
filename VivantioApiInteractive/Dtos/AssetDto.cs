namespace VivantioApiInteractive.Dtos;

internal record AssetDto
{
    public required string AssetTag { get; init; }
    public required string SerialNumber { get; init; }
    public DateTime? PurchaseDate { get; init; }
    public decimal? PurchasePrice { get; init; }
    public DateTime? WarrantyExpiry { get; init; }
    public required int StatusId { get; init; }
    public string? ExternalKey { get; init; }
    public string? ExternalSource { get; init; }
    public string? Notes { get; init; }
    public required int RecordTypeId { get; init; }
}
