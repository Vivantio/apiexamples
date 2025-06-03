using System;
using System.Threading.Tasks;
using VivantioApi.Model;
using static VivantioApi.Data.CustomForms;

namespace CustomForms.Con
{
    internal static class Program
    {
        private const string valueSeparator = "  |  ";
        private const string sectionSeparator = "--------------------------------------------------";

        private static void Main()
        {
            Console.WriteLine(sectionSeparator);
            PrintTicketTypes().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            PrintCustomFormDefinitionsForTicketType().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            PrintCustomFormDefinitionDetailForTicketType().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            PrintCustomFormDefinitionsForTicketInstance().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            PrintCustomFieldDefinition().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            PrintCustomFormInstanceForTicketInstance().GetAwaiter().GetResult();
            Console.WriteLine(sectionSeparator);

            Console.ReadLine();
        }

        private static async Task PrintTicketTypes()
        {
            var ticketTypes = await GetTicketTypeAsync().ConfigureAwait(false);

            foreach (TicketType tt in ticketTypes)
            {
                Console.WriteLine($"ID: {tt.Id}{valueSeparator}Name: {tt.NameSingular}");
            }
        }

        private static async Task PrintCustomFormDefinitionsForTicketType()
        {
            // supply a value returned from PrintTicketTypes()
            const int ticketTypeId = 54;

            var cfds = await GetCustomFormDefinitionForTicketTypeAsync(ticketTypeId).ConfigureAwait(false);

            foreach (FormDefinition cfd in cfds)
            {
                Console.WriteLine($"ID: {cfd.Id}{valueSeparator}Name: {cfd.Name}");
            }
        }

        private static async Task PrintCustomFormDefinitionDetailForTicketType()
        {
            // supply a value returned from PrintTicketTypes()
            const int formDefinitionId = 9799;

            var fds = await GetCustomFormDefinitionDetailForFormDefinitionIdAsync(formDefinitionId).ConfigureAwait(false);

            if (fds != null)
                Console.WriteLine($"ID: {fds.Id}{valueSeparator}Name: {fds.Name}");
        }

        private static async Task PrintCustomFormDefinitionsForTicketInstance()
        {
            const int ticketInstanceId = 655984;
            const string systemArea = "Ticket";

            var cfds = await GetCustomFormDefinitionForTicketInstanceAsync(ticketInstanceId, systemArea).ConfigureAwait(false);

            foreach (string cfd in cfds)
            {
                Console.WriteLine($"ID: {cfd}");
            }
        }

        private static async Task PrintCustomFieldDefinition()
        {
            const int fieldDefinitionId = 13289;

            var fd = await GetCustomFieldDefinitionAsync(fieldDefinitionId).ConfigureAwait(false);

            if (fd != null)
                Console.WriteLine($"ID: {fd.Id}{valueSeparator}Name: {fd.Name}{valueSeparator}Label: {fd.Label}");
        }

        private static async Task PrintCustomFormInstanceForTicketInstance()
        {
            const int typeId = 0;
            const int ticketTypeId = 655984;
            const string systemArea = "Ticket";

            var fis = await GetCustomFormInstanceForTicketInstanceAsync(typeId, ticketTypeId, systemArea).ConfigureAwait(false);

            foreach (FormInstance fi in fis)
            {
                Console.WriteLine($"ID: {fi.Id}{valueSeparator}Definition ID: {fi.CustomEntityDefinitionId}{valueSeparator}Parent ID: {fi.ParentId}");
            }
        }
    }
}
