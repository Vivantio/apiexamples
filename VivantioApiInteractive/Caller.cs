namespace VivantioApiInteractive;

internal static class Caller
{
    public static async Task InsertCallers(ClientId clientId, string clientDomain, int locationId, int numberToInsert = 2)
    {
        var random = RandomProvider.Instance;
        for (int i = 1; i < numberToInsert + 1; i++)
        {
            var randomStrig = RandomStringHelper.GenerateRandomString(1, random);
            var name = $"{RandomStringHelper.GetRandomFirstName(random)} {randomStrig.ToUpper()} {RandomStringHelper.GetRandomLastName(random)}";
            //var name = "intentional duplicate for testing";
            var email = $"{name.ToLower().Replace(" ", ".")}@{clientDomain}";

            var caller = new CallerDto
            {
                Name = name,
                Email = email,
                Phone = "01234 567890",
                DomainLoginName = email,
                ClientId = clientId.Value,
                LocationId = locationId,
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
                await Asset.InsertAssetReleation(insertedAssetIds.ToList(), locationId, SystemArea.Location);
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
