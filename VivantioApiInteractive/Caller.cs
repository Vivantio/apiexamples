namespace VivantioApiInteractive;

internal static class Caller
{
    public static async Task InsertCallers(ClientId clientId, string clientDomain, LocationId locationId, int numberToInsert = 2)
    {
        var random = RandomProvider.Instance;
        for (int i = 1; i < numberToInsert + 1; i++)
        {
            var name = $"{Faker.Name.First()} {Faker.Name.Last()}";
            var email = $"{name.ToLower().Replace(" ", ".")}@{clientDomain}";

            var caller = new CallerDto
            {
                Name = name,
                Email = email,
                Phone = Faker.Phone.Number(),
                DomainLoginName = email,
                ClientId = clientId.Value,
                LocationId = locationId.Value,
                SelfServiceLoginEnabled = true,
                Notes = RandomStringHelper.GetLoremIpsum(),
                ExternalKey = $"ext-{email}",
                ExternalSource = AppHelper.ExternalSource,
                RecordTypeId = (int)RecordType.Caller,
            };

            var response = await ApiHelper.SendRequestAsync<InsertResponse, CallerDto>("Caller/Insert", caller);

            if (response != null && response.Successful)
            {

                var insertedCallerId = new CallerId { Value = response.InsertedItemId };

                // Callers can have attachments
                var identifierText = "a caller";
                var fileContentText = $"This attachment was created for Caller {name}";
                await Attachment.InsertAttachment(SystemArea.Caller, insertedCallerId.Value, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment(SystemArea.Caller, insertedCallerId.Value, AttachmentFileType.Text, identifierText, fileContentText, 2);

                // Callers can have assets but these also belong to the Client and Have a Location, so we insert personal assets and link them to the Client, Caller, and Location
                var insertedAssetIds = await Asset.InsertPersonalAssets();
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), clientId.Value, SystemArea.Client);
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), insertedCallerId.Value, SystemArea.Caller);
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), locationId.Value, SystemArea.Location);
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
