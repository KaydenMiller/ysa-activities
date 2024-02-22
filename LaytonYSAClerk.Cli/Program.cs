using System.Text;
using System.Text.Json;using Alba.CsConsoleFormat;
using LaytonYSAClerk.Cli;

var file = File.ReadAllText("./merged-records.json");
var mergeRecords = JsonSerializer.Deserialize<List<MergedRecord>>(file)!;

var sortedRecords = mergeRecords
    .OrderBy(x => x.Fullname)
    .ToList();

var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Single);

var document = new Document(
    new Grid
    {
        Color = ConsoleColor.Gray,
        Stroke = headerThickness,
        Columns =
        {
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
        },
        Children =
        {
            new Cell("Fullname") { Stroke = headerThickness },
            new Cell("Pulled") { Stroke = headerThickness },
            new Cell("Pulled Timestamp") { Stroke = headerThickness },
            new Cell("Birthday") { Stroke = headerThickness },
            // new Cell("Email") { Stroke = headerThickness },
            new Cell("Phone") { Stroke = headerThickness },
            new Cell("Address") { Stroke = headerThickness },
            new Cell("Apartment") { Stroke = headerThickness },
            new Cell("City") { Stroke = headerThickness },
            new Cell("Zip") { Stroke = headerThickness },
            // new Cell("Photo") { Stroke = headerThickness },
            sortedRecords.Select(item => new[]
            {
                new Cell(item.Fullname) { MaxWidth = 25 },
                new Cell(item.Pulled),
                new Cell(item.PulledTimestamp),
                new Cell(item.Birthday),
                // new Cell(item.Email),
                new Cell(item.Phone),
                new Cell(item.Address),
                new Cell(item.ApartmentNumber),
                new Cell(item.City),
                new Cell(item.Zip),
                // new Cell(item.PhotoDriveUrl),
            })
        }
    });

ConsoleRenderer.RenderDocument(document);

var textOutput = JsonSerializer.Serialize(sortedRecords, new JsonSerializerOptions()
{
    WriteIndented = true,
});
File.WriteAllText("./sorted-records.json", textOutput, Encoding.UTF8);
