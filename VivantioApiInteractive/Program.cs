namespace VivantioApiInteractive;

internal class Program
{
    static async Task Main()
    {
        ShowFiglet();

        // Check environment variables and connection to API
        if (!Helper.AreEnvVarsSet())
        {
            ShowEnvironmentSetup();
            AnsiConsole.Prompt(new TextPrompt<string>(Spectre.ExitMessage).AllowEmpty());
            return;
        }
        else if (!await Helper.CanConnect())
        {
            AnsiConsole.MarkupLine("[red]Cannot connect to Vivantio API. Please check your environment variables.[/]");
            AnsiConsole.Prompt(new TextPrompt<string>(Spectre.ExitMessage).AllowEmpty());
            return;
        }

        var items = new[]
            {
                (Name: "Environment variables configured", Checked: true),
                (Name: $"Successful connection to API at {Helper.GetPlatformUrl()}", Checked: true)
            };

        foreach (var item in items)
        {
            var symbol = item.Checked ? "[green][[x]][/]" : "[grey][[ ]][/]";
            AnsiConsole.MarkupLine($"{symbol} [green]{item.Name}[/]");
        }
        AnsiConsole.WriteLine();

        Spectre.EnterToContinue();

        await ShowMainMenu();

    }

    static void ShowFiglet()
    {
        AnsiConsole.Clear();

        // Figlet banner
        AnsiConsole.Write(new FigletText(Helper.ApplicationName).LeftJustified().Color(Color.Green));

        AnsiConsole.WriteLine();

        // Rule (horizontal line)
        AnsiConsole.Write(new Rule($"[bold green]Welcome to {Helper.ApplicationName}[/]").LeftJustified());

        AnsiConsole.WriteLine();

    }

    static void ShowEnvironmentSetup()
    {
        AnsiConsole.Write(
        new Panel(
            "[green]Please set API Credential environment variables as shown in Vivantio > Admin > Downloads.[/]\n" +
            "[green]Using PowerShell for example:[/]\n\n" +
            "[blue][[System.Environment]]::SetEnvironmentVariable(\"VIVANTIO_PLATFORM_URL\", \"<Platform Url>\", \"User\")\n" +
            "[[System.Environment]]::SetEnvironmentVariable(\"VIVANTIO_USERNAME\", \"<Username>\", \"User\")\n" +
            "[[System.Environment]]::SetEnvironmentVariable(\"VIVANTIO_PASSWORD\", \"<Password>\", \"User\")[/]\n\n" +
            "[green]Note that you may need to restart Visual Studio for values to be recognised.[/]\n"
        )
        .Border(BoxBorder.Rounded)
        .Header("[bold red]Environment Setup[/]", Justify.Left)
        .Padding(1, 1)
        .Expand());

        AnsiConsole.WriteLine();
    }
    static async Task ShowMainMenu()
    {
        bool exit = false;

        while (!exit)
        {
            AnsiConsole.Clear();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a Vivantio Record Type")
                    .PageSize(10)
                    .AddChoices(Spectre.MainMenuClients, Spectre.MainMenuTickets, Spectre.MainMenuExit));

            switch (choice)
            {
                case Spectre.MainMenuClients:
                    await Client.ShowMenu();
                    break;
                case Spectre.MainMenuTickets:
                    await Ticket.ShowMenu();
                    break;
                case Spectre.MainMenuExit:
                    exit = true;
                    break;
            }
        }
    }
}
