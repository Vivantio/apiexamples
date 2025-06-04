namespace VivantioApiInteractive
{
    public class SelectResponse<T> : BaseResponse
    {
        public List<T> Results { get; set; }

        public SelectResponse()
        {
            Results = [];
        }
    }
}