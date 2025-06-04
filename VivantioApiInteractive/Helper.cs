using Newtonsoft.Json;
using Spectre.Console;
using System.Text;

namespace VivantioApiInteractive
{
    public static class Helper
    {
        public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
        public const string UsernameEnvVar = "VIVANTIO_USERNAME";
        public const string PasswordEnvVar = "VIVANTIO_PASSWORD";
        public const string ApplicationName = "Vivantio APi Interactive";


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

        public static void Pause()
        {
            AnsiConsole.MarkupLine("[green]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        public static string StripMarkup(string input)
        {
            // Simple workaround to extract raw label (remove [green] and [/])
            return input.Replace("[green]", "").Replace("[/]", "");
        }
    }
}
