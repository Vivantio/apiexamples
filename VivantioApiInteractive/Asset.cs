namespace VivantioApiInteractive;

internal class Asset
{
    public static async Task InsertAssetReleations(List<int> assetIds, List<int> parentItemIds, SystemAreaId systemAreaId)
    {
        var assetRelations = new AssetRelationsDto
        {
            AssetIds = assetIds,
            ParentItemIds = parentItemIds,
            ParentSystemArea = (int)systemAreaId,
            Notes = RandomStringHelper.GetLoremIpsum()
        };
        await ApiHelper.SendRequestAsync<InsertResponse, AssetRelationsDto>("Asset/AssetRelationInsert", assetRelations);
    }

    public static async Task InsertAssetReleation(List<int> assetIds, int parentItemId, SystemAreaId systemAreaId)
    {
        var assetRelation = new AssetRelationDto
        {
            AssetIds = assetIds,
            ParentItemId = parentItemId,
            ParentSystemArea = (int)systemAreaId,
            Notes = RandomStringHelper.GetLoremIpsum()
        };

        await ApiHelper.SendRequestAsync<InsertResponse, AssetRelationDto>("Asset/AssetRelationInsert", assetRelation);
    }

    public static async Task<int[]> InsertPersonalAssets()
    {
        var personalAssets = new[] { "Laptop", "Smartphone", "Tablet", "Monitor", "Printer" };
        var random = RandomProvider.Instance;
        var randomBatchIdentifier = RandomStringHelper.GenerateRandomAlphaNumeric(4, random);
        var assetIds = new List<int>();

        for (int i = 0; i < personalAssets.Length; i++)
        {
            var personalAsset = personalAssets[i];
            var serialNumberAssetTag = $"{personalAsset}-{randomBatchIdentifier}-{i + 1}";
            //var serialNumberAssetTag = "intentional duplicate for testing";
            var randomMultiplier = random.Next(1, 100);
            var randomValue = random.Next(1, 10);
            var warrantyExpiry = randomValue == 1 ? (DateTime?)null : DateTime.Today.AddYears(1 + (i % 3)); // Randomly decide if the attachment is private
            var asset = new AssetDto
            {
                RecordTypeId = 5, // 5 is typically the RecordTypeId for Assets
                StatusId = 7, // 7 = In Use
                SerialNumber = serialNumberAssetTag,
                AssetTag = serialNumberAssetTag,
                PurchaseDate = DateTime.Today.AddDays(-(i + 1)),
                PurchasePrice = (decimal?)(randomMultiplier * (12.33)),
                WarrantyExpiry = warrantyExpiry,
                ExternalKey = $"ext-{serialNumberAssetTag}".ToLower(),
                ExternalSource = AppHelper.ExternalSource,
                Notes = RandomStringHelper.GetLoremIpsum()
            };

            int insertedAssetId;
            var response = await ApiHelper.SendRequestAsync<InsertResponse, AssetDto>("Asset/Insert", asset);

            if (response != null && response.Successful)
            {

                insertedAssetId = response?.InsertedItemId ?? 0;

                assetIds.Add(insertedAssetId);
                var identifierText = "an asset";
                var fileContentText = $"This attachment was created for Asset Tag {serialNumberAssetTag}";
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.Text, identifierText, fileContentText, 2);
            }
            else
            {
                var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                AnsiConsole.MarkupLine($"[red]Error inserting Asset: {errorMessage}[/]");
                continue; // Abort this iteration and continue with the next
            }
        }
        return assetIds.ToArray();
    }

    public static async Task<int[]> InsertCorporateAssets(int numberToInsert = 2)
    {
        if (numberToInsert <= 0)
            throw new ArgumentOutOfRangeException(nameof(numberToInsert), "Number to insert must be positive.");

        var random = new Random();
        var randomBatchIdentifier = RandomStringHelper.GenerateRandomAlphaNumeric(4, random);

        var assetIds = new List<int>();

        for (int i = 0; i < numberToInsert; i++)
        {
            var randomAssetName = RandomStringHelper.GetRandomCorporateAssetName(random);
            var serialNumberAssetTag = $"{randomAssetName}-{randomBatchIdentifier}-{i + 1}";
            var randomMultiplier = random.Next(1, 1000);
            var randomValue = random.Next(1, 10);
            var warrantyExpiry = randomValue == 1 ? (DateTime?)null : DateTime.Today.AddYears(1 + (i % 3)); // Randomly decide if the attachment is private
            var asset = new AssetDto
            {
                RecordTypeId = 5,
                StatusId = 7,
                SerialNumber = serialNumberAssetTag,
                AssetTag = serialNumberAssetTag,
                PurchaseDate = DateTime.Today.AddDays(-(numberToInsert - (i + 1))),
                PurchasePrice = (decimal?)(randomMultiplier * (1233.33)),
                WarrantyExpiry = warrantyExpiry,
                ExternalKey = $"ext-{serialNumberAssetTag}".ToLower(),
                ExternalSource = AppHelper.ExternalSource,
                Notes = RandomStringHelper.GetLoremIpsum()
            };

            int insertedAssetId;
            var response = await ApiHelper.SendRequestAsync<InsertResponse, AssetDto>("Asset/Insert", asset);

            if (response != null && response.Successful)
            {

                insertedAssetId = response?.InsertedItemId ?? 0;

                assetIds.Add(insertedAssetId);
                var identifierText = "an asset";
                var fileContentText = $"This attachment was created for Asset Tag {serialNumberAssetTag}";
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.Text, identifierText, fileContentText, 2);
            }
            else
            {
                var errorMessage = response?.ErrorMessages?.FirstOrDefault()?.ToString() ?? "Unknown error";
                AnsiConsole.MarkupLine($"[red]Error inserting Asset: {errorMessage}[/]");
                continue; // Abort this iteration and continue with the next
            }
        }
        return assetIds.ToArray();
    }
}
