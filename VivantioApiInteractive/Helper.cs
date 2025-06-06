namespace VivantioApiInteractive
{
    public static class Helper
    {
        public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
        public const string UsernameEnvVar = "VIVANTIO_USERNAME";
        public const string PasswordEnvVar = "VIVANTIO_PASSWORD";
        public const string ApplicationName = "Vivantio API Interactive";


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
            var response = await HttpClientProvider.Client.PostAsync("Configuration/TicketTypeSelectAll", null);
            return response.IsSuccessStatusCode;
        }
    }
}
