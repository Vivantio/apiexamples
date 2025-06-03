using System;
using System.Threading.Tasks;
using VivantioApi.Model;
using static VivantioApi.Data.Tickets;

namespace Tickets.Con
{
    internal static class Program
    {
        private const string valueSeparator = "  |  ";
        private const string sectionSeparator = "--------------------------------------------------";
        private static void Main()
        {
            Console.WriteLine(sectionSeparator);
            PrintAllUsers().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);
            PrintSingleTicketDetails().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);
            PrintAllCustomActionsByTicket().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);
            PrintAllChildTicketsById().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);
            PrintMultipleTickets().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);
            Console.ReadLine();
        }

        private static async Task PrintSingleTicketDetails()
        {
            const int ticketId = 10225;
            var std = await GetSingleTicketByIdAsync(ticketId).ConfigureAwait(false);
            if (std != null)
            {
                Console.WriteLine($"ID: {std.Id}{valueSeparator}Title: {std.Title}{valueSeparator} " +
                    $"Open Date: {std.OpenDate}{valueSeparator} Caller Name: {std.CallerName}{valueSeparator} Description: {std.Description}{valueSeparator} Owner Name: {std.CallerName} ");
            }
        }
        private static async Task PrintAllUsers()
        {
            var usersList = await GetAllUsers().ConfigureAwait(false);
            foreach (UserInfo user in usersList)
            {
                Console.WriteLine($"ID: {user.Id}{valueSeparator}Full Name: {user.FirstName + " " + user.LastName}{valueSeparator} Email: {user.EmailAddress}");
            }
        }
        private static async Task PrintAllCustomActionsByTicket()
        {
            const int ticketTypeId = 10370;
            const bool includePrivate = false;
            const bool includeAttachements = false;
            var ticketActions = await GetAllActionsByTicketId(ticketTypeId, includePrivate, includeAttachements).ConfigureAwait(false);
            foreach (TicketAction ta in ticketActions)
            {
                Console.WriteLine($"ID: {ta.Id}{valueSeparator}Description: {ta.Description}{valueSeparator} Action Date: {ta.ActionDate}{valueSeparator} Action UserName: {ta.ActionUserName}{valueSeparator} Action User Email :{ta.ActionUserEmail}");
            }
        }

        private static async Task PrintAllChildTicketsById()
        {
            const int ticketId = 10225;
            var childTickets = await GetChildTicketById(ticketId).ConfigureAwait(false);
            foreach (TicketType ct in childTickets)
            {
                Console.WriteLine($"ID: {ct.Id}{valueSeparator}Title: {ct.Title}{valueSeparator} " +
                $"Open Date: {ct.OpenDate}{valueSeparator} Caller Name: {ct.CallerName}{valueSeparator} Description: {ct.Description}{valueSeparator} Owner Name: {ct.CallerName} ");
            }
        }

        private static async Task PrintMultipleTickets()
        {
            var tickets = new[] { 10404, 10402, 10225 };
            var multipleTickets = await GetListofTickets(tickets).ConfigureAwait(false);
            foreach (TicketType mt in multipleTickets)
            {
                Console.WriteLine($"ID: {mt.Id}{valueSeparator}Title: {mt.Title}{valueSeparator}Open Date: {mt.OpenDate}{valueSeparator} Caller Name: {mt.CallerName}{valueSeparator}Description: {mt.Description}{valueSeparator} Owner Name: {mt.CallerName} ");
            }
        }
    }
}
