// DynamicBarcodeType — select the barcode format and value at runtime via CLI args.
//
// Run:  dotnet run -c Debug-DrawingCompat
//   or: dotnet run -c Debug-DrawingCompat -- QrCode "https://example.com"
//   or: dotnet run -c Debug-DrawingCompat -- BarCode128 "HELLO-WORLD"
//   or: dotnet run -c Debug-DrawingCompat -- DataMatrix "DataMatrix text"
//   or: dotnet run -c Debug-DrawingCompat -- AztecCode "Aztec text"
//   or: dotnet run -c Debug-DrawingCompat -- Pdf417 "PDF417 text"
//   or: dotnet run -c Debug-DrawingCompat -- BarCode39 "CODE39"
//
// Output: barcode-<Type>.pdf in the output directory
//
// Key patterns shown:
//   - <Type>={?BarcodeType}</Type> — format selected at runtime via report parameter
//   - Code property expression ={?BarcodeValue} — value also passed as parameter
//   - Both type and content change without touching the RDL file

using Majorsilence.Reporting.Rdl;

var barcodeType  = args.Length > 0 ? args[0] : "QrCode";
var barcodeValue = args.Length > 1 ? args[1] : "https://github.com/majorsilence/Reporting";

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath = Path.Combine(baseDir, "BarcodeDemo.rdl");
var outPath = Path.Combine(baseDir, $"barcode-{barcodeType}.pdf");

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

var parameters = new Dictionary<string, string>
{
    ["BarcodeType"]  = barcodeType,
    ["BarcodeValue"] = barcodeValue,
};
await report.RunGetData(parameters);

var ofs = new OneFileStreamGen(outPath, true);
await report.RunRender(ofs, OutputPresentationType.PDF);

Console.WriteLine($"Written: {outPath}");
return 0;
