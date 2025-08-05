namespace VivantioApiInteractive.Utility;

internal class Response
{
    public bool Successful { get; set; }
}

internal class SelectResponse<T> : BaseResponse
{
    public List<T> Results { get; set; }

    public SelectResponse()
    {
        Results = [];
    }
}

internal class SelectRequest
{
    public required Query Query { get; set; }
}

internal class InsertResponse : BaseResponse
{
    public int InsertedItemId { get; set; }
}

internal class BaseResponse
{
    public bool Successful { get; set; }
    public List<ErrorMessage>? ErrorMessages { get; set; }
    public List<InfoMessage>? InfoMessages { get; set; }
}

internal class InfoMessage
{
    public InfoType Type { get; set; }

    public string? Field { get; set; }

    public string? Message { get; set; }

    public override string ToString()
    {
        return $"Info Type: {Type}; Message: {Message}; Field: {Field}";
    }
}

internal class ErrorMessage
{
    public ErrorType Type { get; set; }
    public string? Field { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return $"Error Type: {Type}; Message: {Message}; Field: {Field}";
    }
}
