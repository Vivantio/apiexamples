using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace VivantioApi.Data
{
    public static class HttpClientProvider
    {
        public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
        public const string UsernameEnvVar = "VIVANTIO_USERNAME";
        public const string PasswordEnvVar = "VIVANTIO_PASSWORD";

        static HttpClientProvider()
        {
            Client = new HttpClient();

            var apiUrl = $"{Environment.GetEnvironmentVariable(PlatformUrlEnvVar, EnvironmentVariableTarget.User)}api/";
            var apiLogin = Environment.GetEnvironmentVariable(UsernameEnvVar, EnvironmentVariableTarget.User);
            var apiPassword = Environment.GetEnvironmentVariable(PasswordEnvVar, EnvironmentVariableTarget.User);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiLogin + ":" + apiPassword));

            Client.Timeout = TimeSpan.FromMinutes(3);
            Client.BaseAddress = new Uri(apiUrl);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        public static HttpClient Client { get; }
    }
}
