using Spectre.Console;

namespace VivantioApiInteractive
{
    public class LocationDto
    {
        public required string Name { get; set; }
        public required int ClientId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public string? ExternalKey { get; set; }
        public string? ExternalSource { get; set; }
    }

    public static class Location
    {
        public static async Task<int[]> InsertLocations(int clientId, int numberToInsert = 2)
        {
            var random = RandomProvider.Instance;
            var locationIds = new List<int>();
            for (int i = 1; i < numberToInsert + 1; i++)
            {
                var randomLocationName = Helper.GetRandomCityName(random);
                var randomStrig = Helper.GenerateRandomString(2, random);

                var locationName = $"{randomLocationName}-{randomStrig}-{i}";
                var location = new LocationDto
                {
                    Name = locationName,
                    ClientId = clientId,
                    Address1 = "Address 1 text",
                    Address2 = "Address 2 text",
                    Address3 = "Address 3 text",
                    City = "City text",
                    County = "County text",
                    PostCode = "PostCode text",
                    Country = "Country text",
                    Phone = "01234 567890",
                    Notes = Helper.GetLoremIpsum(),
                    ExternalKey = $"ext-{locationName}".ToLower(),
                    ExternalSource = Helper.ExternalSource
,
                };

                int insertedLocationId;
                var response = await ApiUtility.SendRequestAsync<InsertResponse, LocationDto>("Location/Insert", location);

                if (response != null && response.Successful)
                {
                    insertedLocationId = response?.InsertedItemId ?? 0;

                    locationIds.Add(insertedLocationId);
                    var identifierText = "a location";
                    var fileContentText = $"This attachment was created for Location {locationName}";
                    await Attachment.InsertAttachment((int)SystemAreaId.Location, insertedLocationId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                    await Attachment.InsertAttachment((int)SystemAreaId.Location, insertedLocationId, AttachmentFileType.Text, identifierText, fileContentText, 2);

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
}
