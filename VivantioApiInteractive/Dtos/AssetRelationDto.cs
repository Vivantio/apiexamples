namespace VivantioApiInteractive.Dtos;

internal record AssetRelationDto : AssetRelationBaseDto
{
    public required int ParentItemId { get; init; }
}
