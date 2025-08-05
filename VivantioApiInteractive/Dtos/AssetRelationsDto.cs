namespace VivantioApiInteractive.Dtos;

public record AssetRelationsDto : AssetRelationBaseDto
{
    public required List<int> ParentItemIds { get; init; }
}
