namespace VivantioApiInteractive.Dtos;

public record ClientUpdateDto : ClientBaseDto
{
    public required int Id { get; init; }
    public required string Reference { get; init; }
}
