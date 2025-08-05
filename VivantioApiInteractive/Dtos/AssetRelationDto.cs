namespace VivantioApiInteractive.Dtos;

public record AssetRelationDto : AssetRelationBaseDto
{
    public required int ParentItemId { get; init; }
}
