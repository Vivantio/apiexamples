namespace VivantioApiInteractive;

public class Response
{
    public bool Successful { get; set; }
}

public class SelectResponse<T> : BaseResponse
{
    public List<T> Results { get; set; }

    public SelectResponse()
    {
        Results = [];
    }
}

public class SelectRequest
{
    public required Query Query { get; set; }
}

public class InsertResponse : BaseResponse
{
    public int InsertedItemId { get; set; }
}

public class BaseResponse
{
    public bool Successful { get; set; }
    public List<ErrorMessage>? ErrorMessages { get; set; }
    public List<InfoMessage>? InfoMessages { get; set; }
}

public class InfoMessage
{
    public InfoType Type { get; set; }

    public string? Field { get; set; }

    public string? Message { get; set; }

    public override string ToString()
    {
        return $"Info Type: {Type}; Message: {Message}; Field: {Field}";
    }
}

public class ErrorMessage
{
    public ErrorType Type { get; set; }
    public string? Field { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return $"Error Type: {Type}; Message: {Message}; Field: {Field}";
    }
}
