namespace VivantioApiInteractive.Dtos;

internal record AssetRelationsDto : AssetRelationBaseDto
{
    public required List<int> ParentItemIds { get; init; }
}
