namespace VivantioApiInteractive.Utility;

public class Query
{
    [JsonConverter(typeof(StringEnumConverter))]
    public QueryMode Mode { get; set; }
    public List<QueryItem> Items { get; private set; }

    public Query()
    {
        Items = [];
    }
}

public class QueryItem
{
    public required string FieldName { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public Operator Op { get; set; }
    public required object Value { get; set; }
}
