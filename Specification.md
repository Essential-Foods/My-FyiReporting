# Majorsilence.Reporting RDL XML Specification


Draft Date: December 28, 2025


## Overview

This document specifies the structure and elements of the Report Definition Language (RDL) XML files supported by the `Majorsilence.Reporting` engine. The RDL file describes the layout, data sources, parameters, and rendering instructions for a report.

## Schema / targetNamespace

- Primary XSD (implementation reference): `jekyll_site/schemas/reporting/2025/12/reportdefinition/ReportDefinition.xsd`
- targetNamespace used by the schema: `http://reporting.majorsilence.com/schemas/reporting/2025/12/reportdefinition`
- The schema uses `elementFormDefault="qualified"` — elements must be in the target namespace when validating.

When authoring or validating reports for this engine prefer the schema above (or a compatible RDL variant). Note: the engine supports a number of extensions not present in the canonical Microsoft RDL schemas — these are documented below.

## Root Element

- `<Report>`: The root element. All other elements are children of this node. Use the RDL namespace in documents that target the Majorsilence schema (for example: `xmlns="http://reporting.majorsilence.com/schemas/reporting/2025/12/reportdefinition"`) so the file can be validated against the XSD shipped in `jekyll_site`.

## Child Elements

- `<Description>`: (Optional) Text describing the report.
- `<Author>`: (Optional) The report author.
- `<AutoRefresh>`: (Optional) Integer, page auto-refresh interval in seconds.
- `<DataSources>`: (Optional) Contains one or more `<DataSource>` elements. Required only if the report references external data connections.
- `<DataSets>`: (Optional) Contains one or more `<DataSet>` elements. Required only if the report defines datasets.
- `<ReportParameters>`: (Optional) Contains one or more `<ReportParameter>` elements. See details below.
- `<PageWidth>`: (Recommended) The width of the report page (e.g., `8.5in`).
- `<PageHeight>`: (Recommended) The height of the report page (e.g., `11in`).
- `<LeftMargin>`, `<RightMargin>`, `<TopMargin>`, `<BottomMargin>`: (Optional) Margins. Engine defaults may vary; specify explicitly in the report for portability.
- `<EmbeddedImages>`: (Optional) Contains one or more `<EmbeddedImage>` elements.
- `<Language>`: (Optional) The primary language (default: server or engine language if not specified).
- `<Code>`: (Optional) Embedded VB.NET code. (The standard RDL format historically supports VB.NET for embedded code.)
- `<CodeModules>`: (Optional) List of external code modules to reference.
- `<Classes>`: (Optional) Custom extension used by Majorsilence.Reporting to instantiate classes at report-load time. This is not part of the standard RDL schema and will fail strict RDL XSD validation unless your processor explicitly supports the extension. See "Engine-specific extensions" below.
- `<DataTransform>`: (Optional) Path to a data transformation (XSLT).
- `<DataSchema>`: (Optional) Data schema or namespace.
- `<DataElementName>`: (Optional) Name for the top-level data element (default: `Report`).
- `<DataElementStyle>`: (Optional) Rendering style for data elements (engine-specific; e.g., `AttributeNormal`).
- `<Body>`: (Required) Contains the main report items and sizing. The `<Body>` element typically contains a `<Height>` and a `<Width>` which describe the body (printable) area. Note: page size is defined by `<PageWidth>`/`<PageHeight>` on the report root; body width/height live under `<Body>`.

## ReportParameters

- `<ReportParameters>`: (Optional) Container for report parameters.
  - `<ReportParameter Name="...">`: Defines a single parameter.
    - `<DataType>`: (Recommended) The data type (e.g., `String`, `Integer`, `DateTime`).
    - `<Prompt>`: (Optional) The prompt text for the parameter.
    - `<DefaultValues>`: (Optional) The default values container.
      - `<Values>`
        - `<Value>`: (Optional) A single default value.
    - `<ValidValues>`: (Optional) A container for allowed values. For static lists use a simple values collection:
      - `<Values>`
        - `<Value>` (or use `<ParameterValues>` / `<ParameterValue>` with `<Label>` and `<Value>` where supported by the engine).
    - Other parameter-specific elements as supported by the engine.

**Example:**
```xml
<ReportParameters>
  <ReportParameter Name="StartDate">
    <DataType>DateTime</DataType>
    <Prompt>Start Date</Prompt>
    <DefaultValues>
      <Values>
        <Value>2024-01-01</Value>
      </Values>
    </DefaultValues>
  </ReportParameter>
  <ReportParameter Name="Category">
    <DataType>String</DataType>
    <Prompt>Category</Prompt>
    <ValidValues>
      <Values>
        <Value>Books</Value>
        <Value>Electronics</Value>
      </Values>
    </ValidValues>
  </ReportParameter>
</ReportParameters>
```

Notes on parameters: the exact container names (`DefaultValues`/`Values`/`Value`, `ValidValues`/`Values`/`Value` or `ParameterValues`/`ParameterValue`) can vary between RDL versions and engines; validate your documents against the XSD for the RDL version you target.

## Example

A short, corrected example that follows the typical RDL 2008-style layout and uses the RDL namespace:

```xml
<Report xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition">
  <Description>Sample report</Description>
  <Author>majorsilence</Author>
  <AutoRefresh>0</AutoRefresh>
  <DataSources>
    <DataSource Name="DataSource1">
      <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>Data Source=localhost;Initial Catalog=SampleDb;</ConnectString>
      </ConnectionProperties>
    </DataSource>
  </DataSources>
  <DataSets>
    <DataSet Name="DataSet1">
      <Query>
        <DataSourceName>DataSource1</DataSourceName>
        <CommandText>SELECT * FROM SampleTable</CommandText>
      </Query>
    </DataSet>
  </DataSets>
  <ReportParameters>
    <ReportParameter Name="Param1">
      <DataType>String</DataType>
      <Prompt>Parameter 1</Prompt>
    </ReportParameter>
  </ReportParameters>

  <!-- Page size (report-level) -->
  <PageWidth>8.5in</PageWidth>
  <PageHeight>11in</PageHeight>

  <!-- Margins -->
  <LeftMargin>0.5in</LeftMargin>
  <RightMargin>0.5in</RightMargin>
  <TopMargin>0.5in</TopMargin>
  <BottomMargin>0.5in</BottomMargin>

  <EmbeddedImages>
    <EmbeddedImage Name="Logo">
      <MIMEType>image/png</MIMEType>
      <ImageData>...</ImageData>
    </EmbeddedImage>
  </EmbeddedImages>

  <Language>en-US</Language>
  <Code>
    <![CDATA[
      ' VB.NET code here
    ]]>
  </Code>
  <CodeModules>
    <CodeModule>MyCustomAssembly.dll</CodeModule>
  </CodeModules>

  <!-- Custom extension: Classes is NOT standard RDL. Keep it only if your engine supports it. -->
  <Classes>
    <Class Name="MyClass">
      <Instance>
        <InstanceMember Name="MyMember">Value</InstanceMember>
      </Instance>
    </Class>
  </Classes>

  <DataTransform>transform.xslt</DataTransform>
  <DataSchema>http://example.com/schema</DataSchema>
  <DataElementName>Report</DataElementName>
  <DataElementStyle>AttributeNormal</DataElementStyle>

  <Body>
    <!-- Body sizing (use Body/Width for printable area width) -->
    <Width>8in</Width>
    <Height>9in</Height>
    <!-- Report items go here -->
  </Body>
</Report>
```

## Engine-specific extensions and behavior

The Majorsilence.Reporting XSD and implementation include several engine-specific extensions and behaviors not present in every canonical RDL distribution. These are supported by the runtime and definition classes under `RdlEngine/Definition` and `RdlEngine/Runtime`.

Key extensions and notes:

- Inline `<Rows>` inside a `<DataSet>` (engine extension):
  - Purpose: allow embedding sample or static XML rows directly inside a DataSet for testing or offline reports.
  - Where: Definition class `RdlEngine/Definition/Rows.cs` and `StaticRows.cs` (see `DataSetDefn.cs` for parsing). The XSD contains a `Rows`/`StaticRows` type and the implementation contains code comments noting this extension.
  - Usage: `<Rows file="path-or-omit">` … `</Rows>` or embedded `<XmlRowData>` child nodes. If `file` is present the engine will load row data from the file; otherwise use inline XML.

- `<Classes>` and `Class` element instantiation:
  - Purpose: instantiate CLR objects at report load time and make them available for expressions.
  - Where: Definition classes `RdlEngine/Definition/Classes.cs` and `RdlEngine/Definition/ReportClass.cs`. Instances are wired into the runtime `Report` via `Report.Classes`.
  - Notes: assemblies referenced by `CodeModules` should be available to the process; reflection/activator-based instantiation is used.

- `<Code>` and `<CodeModules>` handling:
  - Purpose: embedded VB.NET code and external modules referenced by the report.
  - Where: `RdlEngine/Definition/Code.cs` and `CodeModules.cs`; `Report` initialization in `RdlEngine/Runtime/Report.cs` integrates compiled/external code.
  - Notes: Code execution is sandboxed to the engine's expression evaluator; loading external assemblies requires those assemblies to be resolvable by the hosting process.

- `DataSourceReference` and data source resolution:
  - Purpose: allow references to shared data sources or external data source definitions.
  - Where: `RdlEngine/Definition/DataSourceReference.cs` and `DataSourceDefn.cs`; runtime access in `RdlEngine/Runtime/DataSource.cs` and `DataSources.cs`.

- `CustomReportItem` extensibility:
  - Purpose: allows adding non-standard report items implemented by external assemblies.
  - Where: Definition `RdlEngine/Definition/CustomReportItem.cs` and runtime interface `RdlEngine/Runtime/ICustomReportItem.cs`.
  - Notes: Implementors must supply a runtime that implements `ICustomReportItem` and register it with the engine (see related comments in the source).

- Connection provider resolution and configuration:
  - The `ConnectionProperties` element includes a `DataProvider` string; the engine maps provider strings to data providers via `RdlEngine/Runtime/RdlEngineConfig.cs` and `RdlEngine/Definition/ConnectionProperties.cs`.

## Mapping: XSD elements/types → Definition & Runtime source

This section provides a quick reference mapping from important XSD elements/types to the implementation files (relative to repo root). Use this to locate parsing/definition logic and runtime behavior.

Note: this is a prioritized, non-exhaustive list of the most commonly-used types.

- Report (element)
  - Definition: `RdlEngine/Definition/ReportDefn.cs` (class ReportDefn)
  - Runtime: `RdlEngine/Runtime/Report.cs` (class Report)

- DataSources / DataSource
  - Definition: `RdlEngine/Definition/DataSourcesDefn.cs`, `RdlEngine/Definition/DataSourceDefn.cs`
  - Runtime: `RdlEngine/Runtime/DataSources.cs`, `RdlEngine/Runtime/DataSource.cs`

- ConnectionProperties
  - Definition: `RdlEngine/Definition/ConnectionProperties.cs`

- DataSets / DataSet
  - Definition: `RdlEngine/Definition/DataSetsDefn.cs`, `RdlEngine/Definition/DataSetDefn.cs`
  - Runtime: `RdlEngine/Runtime/DataSets.cs`, `RdlEngine/Runtime/DataSet.cs`

- Query / QueryParameter
  - Definition: `RdlEngine/Definition/Query.cs`, `RdlEngine/Definition/QueryParameters.cs`, `RdlEngine/Definition/QueryParameter.cs`

- Fields / Field
  - Definition: `RdlEngine/Definition/Fields.cs`, `RdlEngine/Definition/Field.cs`

- ReportParameters / ReportParameter
  - Definition: `RdlEngine/Definition/ReportParameters.cs`, `RdlEngine/Definition/ReportParameter.cs`

- Body / ReportItems / ReportItem
  - Definition: `RdlEngine/Definition/Body.cs`, `RdlEngine/Definition/ReportItems.cs`, `RdlEngine/Definition/ReportItem.cs`
  - Runtime (layout/rendering): `RdlEngine/Runtime/Page.cs`, `RdlEngine/Runtime/Pages.cs`, `RdlEngine/Runtime/PageTextHtml.cs`

- Textbox / Image / Subreport
  - Definition: `RdlEngine/Definition/Textbox.cs`, `RdlEngine/Definition/EmbeddedImage.cs`, `RdlEngine/Definition/Subreport.cs`
  - Runtime: page item classes under `RdlEngine/Runtime` (see `PageTextHtml.cs` and related rendering code)

- Table / Matrix / List
  - Definition: `RdlEngine/Definition/Table*.cs` (Table/Rows/Columns etc.), `RdlEngine/Definition/Matrix.cs`, `RdlEngine/Definition/List.cs`

- Chart
  - Definition: `RdlEngine/Definition/Chart.cs` and many chart-specific files (e.g., `ChartBar.cs`, `ChartPie.cs`, `ChartData.cs`, `ChartSeries.cs`, `ChartGridLines.cs`)

- Code / CodeModules
  - Definition: `RdlEngine/Definition/Code.cs`, `RdlEngine/Definition/CodeModules.cs`, `RdlEngine/Definition/CodeModule.cs`
  - Runtime: `RdlEngine/Runtime/Report.cs` integrates code modules at report load

- Classes / Class instantiation
  - Definition: `RdlEngine/Definition/Classes.cs`, `RdlEngine/Definition/ReportClass.cs`
  - Runtime: instantiated and stored in `Report.Classes` (see `RdlEngine/Runtime/Report.cs`)

- EmbeddedImages / EmbeddedImage
  - Definition: `RdlEngine/Definition/EmbeddedImage.cs`
  - Runtime: handled by `Report` at render time (image rendering code lives in `RdlEngine/Runtime`)

- CustomReportItem
  - Definition: `RdlEngine/Definition/CustomReportItem.cs`
  - Runtime interface: `RdlEngine/Runtime/ICustomReportItem.cs`

- Rows / StaticRows (inline XML rows extension)
  - Definition: `RdlEngine/Definition/Rows.cs`, `RdlEngine/Definition/StaticRows.cs`, `RdlEngine/Definition/StaticRow.cs`
  - Runtime: DataSet parsing loads row XML into runtime dataset objects (`RdlEngine/Runtime/DataSet.cs`)

- Filters / Filter
  - Definition: `RdlEngine/Definition/Filters.cs`, `RdlEngine/Definition/Filter.cs`

This mapping is not exhaustive — the `RdlEngine/Definition` directory contains numerous types for styling, grouping, sorting, chart/plot configuration and other fine-grained features. Use the mapping above as the most useful starting point.

## Example snippets (recommended additions to the spec)

Below are concise examples you can paste into reports targeting the Majorsilence schema.

1) Minimal report skeleton (use the Majorsilence target namespace):

```xml
<Report xmlns="http://reporting.majorsilence.com/schemas/reporting/2025/12/reportdefinition">
  <Body>
    <Width>8in</Width>
    <Height>2in</Height>
  </Body>
</Report>
```

2) DataSource with ConnectionProperties (most common case):

```xml
<DataSources>
  <DataSource Name="DS1">
    <ConnectionProperties>
      <DataProvider>SQL</DataProvider>
      <ConnectString>Data Source=.;Initial Catalog=Northwind;</ConnectString>
    </ConnectionProperties>
  </DataSource>
</DataSources>
```

3) DataSet with Query and Fields:

```xml
<DataSet Name="Products">
  <Query>
    <DataSourceName>DS1</DataSourceName>
    <CommandText>SELECT ProductID, ProductName FROM Products</CommandText>
  </Query>
  <Fields>
    <Field Name="ProductID"><DataField>ProductID</DataField></Field>
    <Field Name="ProductName"><DataField>ProductName</DataField></Field>
  </Fields>
</DataSet>
```

4) Inline Rows extension (engine-specific) — embed XML rows or reference a file:

```xml
<DataSet Name="InlineXml">
  <Fields>
    <Field Name="Id"><DataField>Id</DataField></Field>
    <Field Name="Value"><DataField>Value</DataField></Field>
  </Fields>
  <Rows>
    <XmlRowData>
      <Row><Id>1</Id><Value>One</Value></Row>
      <Row><Id>2</Id><Value>Two</Value></Row>
    </XmlRowData>
  </Rows>
</DataSet>
```

5) Classes + CodeModules (instantiate CLR helpers):

```xml
<CodeModules>
  <CodeModule>MyHelpers.dll</CodeModule>
</CodeModules>
<Classes>
  <Class Name="MyHelpers.MathHelper" InstanceName="Math"/>
</Classes>
```

- The `Classes` element is implemented by `RdlEngine/Definition/Classes.cs` and `ReportClass.cs`. At runtime `Report` creates instances that can be referenced from expressions (see `Report.Classes` usage in `RdlEngine/Runtime/Report.cs`).

## Notes and validation guidance

- XML element and attribute names are case-sensitive. Use the exact element and attribute names required by the RDL version you target.
- Required elements: `<Body>` is required for a valid report body. Page size (`<PageWidth>`/`<PageHeight>`) and margins are strongly recommended; if omitted the rendering engine will apply its defaults.
- `<DataSources>` and `<DataSets>` are required only when the report references external data.
- Embedded code is typically VB.NET in standard RDL; external code modules are supported via `<CodeModules>` when the engine allows external assemblies.
- `<Classes>` and similar top-level extensions are not part of the official RDL schema and will fail strict XSD validation against Microsoft schemas; they are supported by this engine as implemented in `RdlEngine/Definition/Classes.cs`.
- Unknown or non-standard elements will cause strict RDL XSD validation to fail. Some renderers may ignore unknown elements at runtime, but you cannot rely on this behavior for portability.
- Always validate your RDL documents against the XSD in `jekyll_site/schemas/reporting/2025/12/reportdefinition/ReportDefinition.xsd` when targeting this codebase, and test reports against the `RdlEngine` implementation.

## Appendix: Implementation files of interest (short list)

- RdlEngine/Definition/ReportDefn.cs
- RdlEngine/Definition/RDLParser.cs
- RdlEngine/Definition/DataSetDefn.cs
- RdlEngine/Definition/DataSourceDefn.cs
- RdlEngine/Definition/Rows.cs
- RdlEngine/Definition/StaticRows.cs
- RdlEngine/Definition/Classes.cs
- RdlEngine/Definition/ReportClass.cs
- RdlEngine/Definition/Code.cs
- RdlEngine/Definition/CodeModules.cs
- RdlEngine/Definition/EmbeddedImage.cs
- RdlEngine/Definition/CustomReportItem.cs
- RdlEngine/Runtime/Report.cs
- RdlEngine/Runtime/DataSet.cs
- RdlEngine/Runtime/DataSource.cs
- RdlEngine/Runtime/ICustomReportItem.cs
- RdlEngine/Runtime/Pages.cs
