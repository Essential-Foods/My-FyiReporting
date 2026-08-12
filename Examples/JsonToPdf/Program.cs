// JsonToPdf — read data from a local JSON file and render a PDF.
//
// Run:  dotnet run
// Output: employees.pdf in the output directory
//
// Key patterns shown:
//   - The Json data provider reads a local .json file as a data source
//   - The JSON file path is injected into the RDL XML before parsing
//   - Nested JSON fields are accessed with underscore notation:
//       Contact.Email → Contact_Email in the RDL field definition
//   - No database required — the JSON file is the entire data source

using Majorsilence.Reporting.Rdl;

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath  = Path.Combine(baseDir, "Employees.rdl");
var jsonPath = Path.Combine(baseDir, "employees.json");
var outPath  = Path.Combine(baseDir, "employees.pdf");

// Inject the absolute JSON file path so the engine can open it at parse time
var rdlXml = (await File.ReadAllTextAsync(rdlPath))
    .Replace("employees.json", jsonPath);

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
