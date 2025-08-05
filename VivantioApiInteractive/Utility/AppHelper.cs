namespace VivantioApiInteractive.Utility;

public static class AppHelper
{
    public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
    public const string UsernameEnvVar = "VIVANTIO_USERNAME";
    public const string PasswordEnvVar = "VIVANTIO_PASSWORD";
    public const string ApplicationName = "Vivantio API Interactive";
    public static readonly string ExternalSource = ApplicationName.Replace(" ", "-").ToLower();


    public static bool AreEnvVarsSet()
    {
        string? platformUrl = Environment.GetEnvironmentVariable(PlatformUrlEnvVar);
        string? username = Environment.GetEnvironmentVariable(UsernameEnvVar);
        string? password = Environment.GetEnvironmentVariable(PasswordEnvVar);

        return !string.IsNullOrWhiteSpace(platformUrl) &&
               !string.IsNullOrWhiteSpace(username) &&
               !string.IsNullOrWhiteSpace(password);
    }

    public static async Task<bool> CanConnect()
    {
        var response = await ApiHelper.SendRequestAsync<BaseResponse>("Configuration/TicketTypeSelectAll");

        if (response == null)
        {
            return false;
        }

        return response.Successful;
    }

    public static string GetPlatformUrl()
    {
        string? platformUrl = Environment.GetEnvironmentVariable(PlatformUrlEnvVar);
        if (string.IsNullOrWhiteSpace(platformUrl))
        {
            throw new InvalidOperationException($"Environment variable '{PlatformUrlEnvVar}' is not set.");
        }
        return platformUrl[..^1];
    }
}




