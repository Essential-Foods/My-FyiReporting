// ExportMultipleFormats — query the database once, render to PDF, Excel, CSV, and HTML.
//
// Run:  dotnet run
// Output: orders.pdf / orders.xlsx / orders.csv / orders.html in the output directory
//
// Key patterns shown:
//   - RunGetData() is called ONCE — the database is only queried once
//   - RunRender() is called once per desired format — each call rebuilds pages
//     from the already-fetched data without touching the database again
//   - OutputPresentationType selects the output format

using Majorsilence.Reporting.Rdl;

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath = Path.Combine(baseDir, "Orders.rdl");
var dbPath  = Path.Combine(baseDir, "sqlitetestdb2.db");

var rdlXml = (await File.ReadAllTextAsync(rdlPath))
    .Replace("sqlitetestdb2.db", dbPath);

var rdlp = new RDLParser(rdlXml) { Folder = baseDir };
using var report = await rdlp.Parse();

if (report.ErrorMaxSeverity > 4)
{
    Console.Error.WriteLine("Report parse errors:");
    foreach (var err in report.ErrorItems)
        Console.Error.WriteLine($"  {err}");
    return 1;
}

// Query the database exactly once
await report.RunGetData();

// Render to each format from the same in-memory data
var formats = new[]
{
    ("orders.pdf",  OutputPresentationType.PDF),
    ("orders.xlsx", OutputPresentationType.Excel2007),
    ("orders.csv",  OutputPresentationType.CSV),
    ("orders.html", OutputPresentationType.HTML),
};

foreach (var (filename, format) in formats)
{
    var outPath = Path.Combine(baseDir, filename);
    var ofs = new OneFileStreamGen(outPath, true);
    await report.RunRender(ofs, format);
    Console.WriteLine($"Written: {outPath}");
}

return 0;
