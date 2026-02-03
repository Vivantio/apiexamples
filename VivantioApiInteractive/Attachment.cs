using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace VivantioApiInteractive;

internal static class Attachment
{
    public static async Task InsertAttachment(SystemArea systemArea, int parentId, AttachmentFileType attchmentType, string identifierText, string fileContentText, int numberToInsert)
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
                content = CreatePdfFile(fileContentTextWithDateTime);
            }

            else if (attchmentType == AttachmentFileType.Text)
            {
                filename = $"This is a text document for {identifierText} {i}.txt";
                content = CreateTextFile(fileContentTextWithDateTime);
            }

            var attachment = new AttachmentDto
            {
                SystemArea = (int)systemArea,
                ParentId = parentId,
                FileName = filename,
                Description = filename,
                Content = content,
                IsPrivate = isPrivate
            };

            try
            {
                await ApiHelper.SendRequestAsync<InsertResponse, AttachmentDto>("File/AttachmentUpload", attachment);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error inserting Attachment: {ex.Message}[/]");
                continue; // Abort this iteration and continue with the next
            }
        }
    }


    public static byte[] CreatePdfFile(string fileContentText)
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

                page.Header().Text($"PDF generated for {AppHelper.ApplicationName}");

                page.Content().Column(col =>
                {
                    col.Item().Text(fileContentText);
                    col.Item().Text("");
                    col.Item().Text(RandomStringHelper.GetLoremIpsum());
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

    public static byte[] CreateTextFile(string fileContentText)
    {
        var output = fileContentText + Environment.NewLine + Environment.NewLine + RandomStringHelper.GetLoremIpsum   ();
        return Encoding.UTF8.GetBytes(output);
    }
}
