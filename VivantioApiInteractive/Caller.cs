using Spectre.Console;

namespace VivantioApiInteractive
{
    public class CallerDto
    {
        public required string Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? DomainLoginName { get; set; }
        public required int ClientId { get; set; }
        public int? LocationId { get; set; }
        public bool SelfServiceLoginEnabled { get; set; }
        public string? Notes { get; set; }
        public string? ExternalKey { get; set; }
        public string? ExternalSource { get; set; }
        public required int RecordTypeId { get; set; }

    }
    public static class Caller
    {
        public static async Task InsertCallers(int clientId, string clientDomain, int locationId, int numberToInsert = 2)
        {
            var random = RandomProvider.Instance;
            for (int i = 1; i < numberToInsert + 1; i++)
            {
                var randomStrig = Helper.GenerateRandomString(1, random);
                var name = $"{Helper.GetRandomFirstName(random)} {randomStrig.ToUpper()} {Helper.GetRandomLastName(random)}";
                //var name = "intentional duplicate for testing";
                var email = $"{name.ToLower().Replace(" ", ".")}@{clientDomain}";

                var caller = new CallerDto
                {
                    Name = name,
                    Email = email,
                    Phone = "01234 567890",
                    DomainLoginName = email,
                    ClientId = clientId,
                    LocationId = locationId,
                    SelfServiceLoginEnabled = true,
                    Notes = Helper.GetLoremIpsum(),
                    ExternalKey = $"ext-{email}",
                    ExternalSource = Helper.ExternalSource,
                    RecordTypeId = 7,
                };

                int insertedCallerId;
                var response = await ApiUtility.SendRequestAsync<InsertResponse, CallerDto>("Caller/Insert", caller);

                if (response != null && response.Successful)
                {

                    insertedCallerId = response?.InsertedItemId ?? 0;
                    // Callers can have attachments
                    var identifierText = "a caller";
                    var fileContentText = $"This attachment was created for Client {name}";
                    await Attachment.InsertAttachment((int)SystemAreaId.Caller, insertedCallerId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                    await Attachment.InsertAttachment((int)SystemAreaId.Caller, insertedCallerId, AttachmentFileType.Text, identifierText, fileContentText, 2);

                    // Callers can have assets but these also belong to the Client and Have a Location, so we insert personal assets and link them to the Client, Caller, and Location
                    var insertedAssetIds = await Asset.InsertPersonalAssets();
                    await Asset.InsertAssetReleation(insertedAssetIds.ToList(), clientId, SystemAreaId.Client);
                    await Asset.InsertAssetReleation(insertedAssetIds.ToList(), insertedCallerId, SystemAreaId.Caller);
                    await Asset.InsertAssetReleation(insertedAssetIds.ToList(), locationId, SystemAreaId.Location);
                }
                else
                {
                    var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                    AnsiConsole.MarkupLine($"[red]Error inserting Caller: {errorMessage}[/]");
                    continue; // Abort this iteration and continue with the next
                }
            }
        }
    }
}
