namespace VivantioApiInteractive
{
    public class ApiResponse
    {
        public int InsertedItemId { get; set; }
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
}
