// BarcodeShowcase — renders all supported CRI barcode formats to a single PDF.
//
// Run:  dotnet run -c Debug-DrawingCompat
//
// Output: barcode-showcase.pdf in the output directory
//
// Key patterns shown:
//   - CustomReportItem with a literal <Type> name from RdlEngineConfig.xml
//   - Static Code values (no database or data binding needed)
//   - Six formats on one page: QrCode, BarCode128, BarCode39, DataMatrix, AztecCode, Pdf417

using Majorsilence.Reporting.Rdl;

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath = Path.Combine(baseDir, "BarcodeShowcase.rdl");
var outPath = Path.Combine(baseDir, "barcode-showcase.pdf");

var rdlXml = await File.ReadAllTextAsync(rdlPath);

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
