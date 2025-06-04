using Newtonsoft.Json;
using System.Text;

namespace VivantioApiInteractive
{
    public class ApiUtility
    {
        public static async Task<TResponse?> SendRequestAsync<TResponse, TRequest>(string path, TRequest? request) where TRequest : class
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await HttpClientProvider.Client.PostAsync(path, content);
            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<TResponse?> SendRequestAsync<TResponse>(string path)
        {
            return await SendRequestAsync<TResponse, object>(path, null);
        }
    }
}