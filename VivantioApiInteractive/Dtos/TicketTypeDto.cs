namespace VivantioApiInteractive.Dtos;

public record TicketTypeDto
{
    public int Id { get; init; }
    public string? NamePlural { get; init; }
    public string? NameSingular { get; init; }
    public bool Enabled { get; init; }
}
