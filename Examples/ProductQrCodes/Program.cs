// ProductQrCodes — product catalog with a QR code per row encoding product details.
//
// Run:  dotnet run -c Debug-DrawingCompat
//
// Output: products-qr.pdf in the output directory
//
// Key patterns shown:
//   - CustomReportItem inside a Table detail row
//   - CRI Code property bound to a data expression using field values
//   - QR code encodes a structured string that could be scanned by any reader

using Majorsilence.Reporting.Rdl;

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath = Path.Combine(baseDir, "ProductsWithQrCode.rdl");
var dbPath  = Path.Combine(baseDir, "sqlitetestdb2.db");
var outPath = Path.Combine(baseDir, "products-qr.pdf");

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

await report.RunGetData();

var ofs = new OneFileStreamGen(outPath, true);
await report.RunRender(ofs, OutputPresentationType.PDF);

Console.WriteLine($"Written: {outPath}");
return 0;
