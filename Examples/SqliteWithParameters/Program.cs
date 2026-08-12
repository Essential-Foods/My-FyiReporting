// SqliteWithParameters — pass report parameters to filter a SQL query.
//
// Run:  dotnet run
//   or: dotnet run -- Germany
//   or: dotnet run -- France
//
// Output: customers-<Country>.pdf in the output directory
//
// Key patterns shown:
//   - Report parameters defined in the RDL are passed as IDictionary<string,string>
//   - The country value is injected into the RDL SQL before Parse() — the same
//     string-replacement pattern used for the DB path avoids parse-time binding issues
//   - The same report file can produce different PDFs by changing the parameter value

using Majorsilence.Reporting.Rdl;

var country = args.Length > 0 ? args[0] : "Germany";

RdlEngineConfig.RdlEngineConfigInit();

var baseDir = AppContext.BaseDirectory;
var rdlPath = Path.Combine(baseDir, "CustomersByCountry.rdl");
var dbPath  = Path.Combine(baseDir, "sqlitetestdb2.db");
var outPath = Path.Combine(baseDir, $"customers-{country}.pdf");

// Inject both the DB path and the country value before Parse() so the engine
// never needs to bind a parameter value it doesn't have at schema-validation time.
var safeCountry = country.Replace("'", "''");
var rdlXml = (await File.ReadAllTextAsync(rdlPath))
    .Replace("sqlitetestdb2.db", dbPath)
    .Replace("'Germany'", $"'{safeCountry}'");

var rdlp = new RDLParser(rdlXml) { Folder = baseDir };
using var report = await rdlp.Parse();

if (report.ErrorMaxSeverity > 4)
{
    Console.Error.WriteLine("Report parse errors:");
    foreach (var err in report.ErrorItems)
        Console.Error.WriteLine($"  {err}");
    return 1;
}

// Pass report parameters as a dictionary — keys match parameter names in the RDL
var parameters = new Dictionary<string, string>
{
    ["Country"] = country
};
await report.RunGetData(parameters);

var ofs = new OneFileStreamGen(outPath, true);
await report.RunRender(ofs, OutputPresentationType.PDF);

Console.WriteLine($"Written: {outPath}");
return 0;
