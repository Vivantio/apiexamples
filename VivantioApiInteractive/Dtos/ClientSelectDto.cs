namespace VivantioApiInteractive.Dtos;

internal record ClientSelectDto : ClientBaseDto
{
    public int Id { get; init; }
    public string? Reference { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? UpdateDate { get; init; }
    public bool? Deleted { get; init; }
    public string? StatusName { get; init; }
}
