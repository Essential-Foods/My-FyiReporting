namespace Majorsilence.Reporting.WebDesigner;

public sealed class RdlDesignerOptions
{
    /// <summary>URL prefix for the designer API endpoints (default: "rdl-designer").</summary>
    public string RoutePrefix { get; set; } = "rdl-designer";

    /// <summary>Folder on disk where RDL files are read and written by the load/save endpoints.</summary>
    public string ReportsFolder { get; set; } = "Reports";

    /// <summary>When false the save endpoint returns 403.</summary>
    public bool AllowSave { get; set; } = true;

    /// <summary>When false the load endpoint returns 403.</summary>
    public bool AllowLoad { get; set; } = true;

    /// <summary>When false the schema-discovery endpoint returns 403.</summary>
    public bool AllowSchema { get; set; } = true;
}
