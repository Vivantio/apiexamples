using Spectre.Console;

namespace VivantioApiInteractive
{
    public class AssetRelationBaseDto
    {
        // The Asset/AssetRelationInsert endpoint expects EITHER a single ParentItemId OR a list of ParentItemIds
        // otherwise an exception is thrown. Splitting the DTO into a base class and derived classes allows us to handle both cases cleanly.
        public required List<int> AssetIds { get; set; }
        public required int ParentSystemArea { get; set; }
        public string? Notes { get; set; }
    }

    public class AssetRelationsDto : AssetRelationBaseDto
    {
        public required List<int> ParentItemIds { get; set; }
    }

    public class AssetRelationDto : AssetRelationBaseDto
    {
        public required int ParentItemId { get; set; }
    }

    public class AssetDto
    {
        public required string AssetTag { get; set; }
        public required string SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public required int StatusId { get; set; }
        public string? ExternalKey { get; set; }
        public string? ExternalSource { get; set; }
        public string? Notes { get; set; }
        public required int RecordTypeId { get; set; }
    }

    public class Asset
    {
        public static async Task InsertAssetReleations(List<int> assetIds, List<int> parentItemIds, SystemAreaId systemAreaId)
        {
            var assetRelations = new AssetRelationsDto
            {
                AssetIds = assetIds,
                ParentItemIds = parentItemIds,
                ParentSystemArea = (int)systemAreaId,
                Notes = Helper.GetLoremIpsum()
            };
            await ApiUtility.SendRequestAsync<InsertResponse, AssetRelationsDto>("Asset/AssetRelationInsert", assetRelations);
        }

        public static async Task InsertAssetReleation(List<int> assetIds, int parentItemId, SystemAreaId systemAreaId)
        {
            var assetRelation = new AssetRelationDto
            {
                AssetIds = assetIds,
                ParentItemId = parentItemId,
                ParentSystemArea = (int)systemAreaId,
                Notes = Helper.GetLoremIpsum()
            };

            await ApiUtility.SendRequestAsync<InsertResponse, AssetRelationDto>("Asset/AssetRelationInsert", assetRelation);
        }

        public static async Task<int[]> InsertPersonalAssets()
        {
            var personalAssets = new[] { "Laptop", "Smartphone", "Tablet", "Monitor", "Printer" };
            var random = RandomProvider.Instance;
            var randomBatchIdentifier = Helper.GenerateRandomAlphaNumeric(4, random);
            var assetIds = new List<int>();

            for (int i = 0; i < personalAssets.Length; i++)
            {
                var personalAsset = personalAssets[i];
                var serialNumberAssetTag = $"{personalAsset}-{randomBatchIdentifier}-{i + 1}";
                var randomMultiplier = random.Next(1, 100);
                var randomValue = random.Next(1, 10);
                var warrantyExpiry = randomValue == 1 ? (DateTime?)null : DateTime.Today.AddYears(1 + (i % 3)); // Randomly decide if the attachment is private
                var asset = new AssetDto
                {
                    RecordTypeId = 5,
                    StatusId = 7,
                    SerialNumber = serialNumberAssetTag,
                    AssetTag = serialNumberAssetTag,
                    PurchaseDate = DateTime.Today.AddDays(-(i + 1)),
                    PurchasePrice = (decimal?)(randomMultiplier * (12.33)),
                    WarrantyExpiry = warrantyExpiry,
                    ExternalKey = $"ext-{serialNumberAssetTag}".ToLower(),
                    ExternalSource = Helper.ExternalSource,
                    Notes = Helper.GetLoremIpsum()
                };

                int insertedAssetId;
                try
                {
                    var response = await ApiUtility.SendRequestAsync<InsertResponse, AssetDto>("Asset/Insert", asset);
                    insertedAssetId = response?.InsertedItemId ?? 0;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error inserting Asset: {ex.Message}[/]");
                    continue; // Abort this iteration and continue with the next
                }

                assetIds.Add(insertedAssetId);
                var identifierText = "an asset";
                var fileContentText = $"This attachment was created for Asset Tag {serialNumberAssetTag}";
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.Text, identifierText, fileContentText, 2);
            }
            return assetIds.ToArray();
        }

        public static async Task<int[]> InsertCorporateAssets(int numberToInsert = 2)
        {
            if (numberToInsert <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberToInsert), "Number to insert must be positive.");

            var random = new Random();
            var randomBatchIdentifier = Helper.GenerateRandomAlphaNumeric(4, random);

            var assetIds = new List<int>();

            for (int i = 0; i < numberToInsert; i++)
            {
                var randomAssetName = Helper.GetRandomCorporateAssetName(random);
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
                    ExternalSource = Helper.ExternalSource,
                    Notes = Helper.GetLoremIpsum()
                };

                int insertedAssetId;
                try
                {
                    var response = await ApiUtility.SendRequestAsync<InsertResponse, AssetDto>("Asset/Insert", asset);
                    insertedAssetId = response?.InsertedItemId ?? 0;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error inserting Asset: {ex.Message}[/]");
                    continue; // Abort this iteration and continue with the next
                }

                assetIds.Add(insertedAssetId);
                var identifierText = "an asset";
                var fileContentText = $"This attachment was created for Asset Tag {serialNumberAssetTag}";
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.PDF, identifierText, fileContentText, 2);
                await Attachment.InsertAttachment((int)SystemAreaId.Asset, insertedAssetId, AttachmentFileType.Text, identifierText, fileContentText, 2);
            }
            return assetIds.ToArray();
        }
    }

}
