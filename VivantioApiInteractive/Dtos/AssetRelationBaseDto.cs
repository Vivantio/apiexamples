namespace VivantioApiInteractive.Dtos;

public record AssetRelationBaseDto
{
    // The Asset/AssetRelationInsert endpoint expects EITHER a single ParentItemId OR a list of ParentItemIds
    // otherwise an exception is thrown. Splitting the DTO into a base class and derived classes allows us to handle both cases cleanly.
    public required List<int> AssetIds { get; init; }
    public required int ParentSystemArea { get; init; }
    public string? Notes { get; init; }
}
