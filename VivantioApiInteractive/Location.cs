namespace VivantioApiInteractive;

internal static class Location
{
    public static async Task<LocationId[]> InsertLocations(ClientId clientId, int numberToInsert = 2)
    {
        var random = RandomProvider.Instance;
        var locationIds = new List<LocationId>();
        for (int i = 1; i < numberToInsert + 1; i++)
        {
            var locationName = Faker.Address.City();
            var location = new LocationDto
            {
                Name = locationName,
                ClientId = clientId.Value,
                Address1 = Faker.Address.SecondaryAddress(),
                Address2 = Faker.Address.StreetAddress(false),
                Address3 = Faker.Address.StreetName(),
                City = locationName,
                County = Faker.Address.UsState(),
                PostCode = Faker.Address.ZipCode(),
                Country = "United States",
                Phone = Faker.Phone.Number(),
                Notes = RandomStringHelper.GetLoremIpsum(),
                ExternalKey = $"ext-{locationName}".ToLower(),
                ExternalSource = AppHelper.ExternalSource
            };

            var response = await ApiHelper.SendRequestAsync<InsertResponse, LocationDto>("Location/Insert", location);

            if (response != null && response.Successful)
            {
                var insertedLocationId = new LocationId { Value = response.InsertedItemId };

                locationIds.Add(insertedLocationId);
                var identifierText = "a location";
                var fileContentText = $"This attachment was created for Location {locationName}";
                await Attachment.InsertAttachment(SystemArea.Location, insertedLocationId.Value, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment(SystemArea.Location, insertedLocationId.Value, AttachmentFileType.Text, identifierText, fileContentText, 2);

            }
            else
            {
                var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                AnsiConsole.MarkupLine($"[red]Error inserting Location: {errorMessage}[/]");
                continue; // Abort this iteration and continue with the next
            }
        }
        return locationIds.ToArray();
    }
}
