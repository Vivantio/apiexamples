using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Spectre.Console;
using System.Text;

namespace VivantioApiInteractive
{
    public class AttachmentDto
    {
        public required int SystemArea { get; set; }
        public required int ParentId { get; set; }
        public required string FileName { get; set; }
        public string? Description { get; set; }
        public required byte[] Content { get; set; }
        public bool? IsPrivate { get; set; }
    }
    public static class Attachment
    {
        public static async Task InsertAttachment(int systemAreaId, int parentId, AttachmentFileType attchmentType, string identifierText, string fileContentText, int numberToInsert)
        {
            var fileContentTextWithDateTime = $"{fileContentText} created on {DateTime.Now}";
            var random = RandomProvider.Instance;

            for (int i = 1; i < numberToInsert + 1; i++)
            {
                var randomValue = random.Next(1, 10);
                var isPrivate = randomValue == 1; // Randomly decide if the attachment is private
                string filename = "text.txt";
                byte[]? content = [];

                if (attchmentType == AttachmentFileType.PDF)
                {
                    filename = $"This is a PDF document for {identifierText} {i}.pdf";
                    content = CreatePdf(fileContentTextWithDateTime);
                }

                else if (attchmentType == AttachmentFileType.Text)
                {
                    filename = $"This is a text document for {identifierText} {i}.txt";
                    content = Encoding.UTF8.GetBytes(fileContentTextWithDateTime);
                }

                var attachment = new AttachmentDto
                {
                    SystemArea = systemAreaId,
                    ParentId = parentId,
                    FileName = filename,
                    Description = filename,
                    Content = content,
                    IsPrivate = isPrivate
                };

                try
                {
                    await ApiUtility.SendRequestAsync<InsertResponse, AttachmentDto>("File/AttachmentUpload", attachment);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error inserting Attachment: {ex.Message}[/]");
                    continue; // Abort this iteration and continue with the next
                }
            }
        }


        public static byte[] CreatePdf(string fileContentText)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            using var stream = new MemoryStream();

            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header().Text($"PDF generated for {Helper.ApplicationName}");

                    page.Content().Column(col =>
                    {
                        col.Item().Text(fileContentText);
                        col.Item().Text(Helper.GetLoremIpsum());
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated ");
                        x.Span(DateTime.Now.ToShortDateString()).SemiBold();
                    });
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }
    }

}
