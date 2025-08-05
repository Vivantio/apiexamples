namespace VivantioApiInteractive
{
    public static class Spectre
    {
        public const string ExitMessage = "Press Enter to exit...";
        public const string ContinueMessage = "Press Enter to continue...";
        public const string MainMenuClients = "Clients";
        public const string MainMenuTickets = "Tickets";
        public const string MainMenuExit = "Exit";
        public const string SubMenuInsert = "Insert";
        public const string SubMenuUpdate = "Update";
        public const string SubMenuBack = "Back to main menu";

        public static void EnterToContinue()
        {
            AnsiConsole.Prompt(new TextPrompt<string>(ContinueMessage).AllowEmpty());
        }
    }
}
