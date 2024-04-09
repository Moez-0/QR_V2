using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Newtonsoft.Json.Linq;
using System;

namespace PDF1
{
    public class Generate65PDF1
    {
        private readonly double paperWidthInMillimeters;
        private readonly double paperHeightInMillimeters;
        private readonly int nbRows;

        [Obsolete]
        public Generate65PDF1(double paperWidthInMillimeters, double paperHeightInMillimeters,int nbRows, JArray dataArray)
        {
            this.paperWidthInMillimeters = paperWidthInMillimeters;
            this.paperHeightInMillimeters = paperHeightInMillimeters;
            this.nbRows = nbRows;
            QuestPDF.Settings.License = LicenseType.Community;
            FontManager.RegisterFont(File.OpenRead("./font/LibreBarcode39-Regular.ttf")); // use file name

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    double paperWidthInPoints = paperWidthInMillimeters * 2.83465; // Convert mm to points
                    double paperHeightInPoints = paperHeightInMillimeters * 2.83465; // Convert mm to points
                    float pageWidthInPoints = (float)(paperWidthInPoints);
                    float pageHeightInPoints = (float)(paperHeightInPoints);
                    page.Size(new PageSize(pageWidthInPoints, pageHeightInPoints)); // Width and height in points

                    page.Content()
                        .Border(1)
                        .Padding(10)
                        .Grid(grid =>
                        {
                            grid.VerticalSpacing(10);
                            grid.HorizontalSpacing(10);
                            grid.AlignCenter();
                            grid.Columns(nbRows); // 12 by default

for (int i = 0; i < dataArray.Count; i++)
{
      var item = dataArray[i];
    string title = item["Title"].ToString();
    string description = item["Description"].ToString();
    string code = item["Code"].ToString();

    // Calculate width and height of each grid item
    double itemWidth = (paperWidthInPoints / 11) - 2; // Divide page width by number of columns and subtract spacing
    double itemHeight = paperHeightInPoints / 6; // 6 rows in total, adjust as needed

    // Calculate maximum available width and height for text
    double maxWidth = itemWidth - 4; // Subtracting padding
    double maxHeight = itemHeight - 4; // Subtracting padding

    // Calculate font sizes based on available space
    double titleFontSize = Math.Min(maxWidth / 10, maxHeight / 5); // Adjust according to your preference
    double descriptionFontSize = Math.Min(maxWidth / 15, maxHeight / 5); // Adjust according to your preference
    double barcodeFontSize = Math.Min(maxWidth / 8, maxHeight / 2); // Adjust according to your preference

    // Create a new grid item with the specified properties
    grid.Item().Background(Colors.White).Border(1).BorderColor(Colors.Black)
    .Width((float)(itemWidth - 2)) // Subtracting margin from width
    .Height(60) // Height of the item
    .Padding(1) // Padding within the item (if needed)
    .AlignCenter() // Align the content to the center
    .Text(text =>
    {
        text.Span(title).Bold().FontSize((float)titleFontSize);
        text.AlignCenter();
        text.EmptyLine();
        text.Span(code).FontFamily("Libre Barcode 39").FontSize((float)barcodeFontSize); // Cast to float

        text.EmptyLine();
        text.Span(description).FontSize((float)descriptionFontSize); // Cast to float
    });
}


                        });
                });
            })
            .GeneratePdf("65pdf.pdf");
        }
    }
}
