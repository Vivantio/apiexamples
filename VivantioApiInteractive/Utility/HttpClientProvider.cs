namespace VivantioApiInteractive.Utility;

internal static class HttpClientProvider
{
    private static readonly HttpClient _client;

    static HttpClientProvider()
    {
        _client = new HttpClient();

        var apiUrl = $"{Environment.GetEnvironmentVariable(AppHelper.PlatformUrlEnvVar, EnvironmentVariableTarget.User)}api/";
        var apiLogin = Environment.GetEnvironmentVariable(AppHelper.UsernameEnvVar, EnvironmentVariableTarget.User);
        var apiPassword = Environment.GetEnvironmentVariable(AppHelper.PasswordEnvVar, EnvironmentVariableTarget.User);

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiLogin + ":" + apiPassword));

        _client.Timeout = TimeSpan.FromMinutes(3);
        _client.BaseAddress = new Uri(apiUrl);
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    public static HttpClient Client => _client;
}
