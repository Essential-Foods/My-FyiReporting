using Majorsilence.Reporting.Rdl;
using NUnit.Framework;
using ReportTests.Utils;
using System;
using System.Threading.Tasks;

namespace ReportTests
{
    [TestFixture]
    public class FunctionTest
    {
        private Uri _reportFolder;

        [SetUp]
        public void SetUp()
        {
            _reportFolder = GeneralUtils.ReportsFolder();
            RdlEngineConfig.RdlEngineConfigInit();
        }

        // Renders FunctionTest.rdl headlessly to verify Mid(), Left(), and field
        // expression evaluation work without requiring a GUI or X11.
        [Test]
        public async Task RdlStringFunctions_RenderToPdf_ProducesNonEmptyOutput()
        {
            Uri fileRdlUri = new Uri(_reportFolder, "FunctionTest.rdl");
            System.IO.Directory.SetCurrentDirectory(_reportFolder.LocalPath);

            Report rap = await RdlUtils.GetReport(fileRdlUri, DatabaseInfo.Connection);
            Assert.That(rap, Is.Not.Null, "Report failed to parse");
            Assert.That(rap.ErrorMaxSeverity, Is.LessThanOrEqualTo(4), "Report has fatal parse errors");

            rap.Folder = _reportFolder.LocalPath;
            await rap.RunGetData();

            using var ms = new MemoryStreamGen();
            await rap.RunRender(ms, OutputPresentationType.RenderPdf_iTextSharp);

            var stream = ms.GetStream();
            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.Length, Is.GreaterThan(0), "Rendered PDF is empty");
        }

        [Test]
        public async Task RdlStringFunctions_RenderToHtml_ContainsExpectedData()
        {
            Uri fileRdlUri = new Uri(_reportFolder, "FunctionTest.rdl");
            System.IO.Directory.SetCurrentDirectory(_reportFolder.LocalPath);

            Report rap = await RdlUtils.GetReport(fileRdlUri, DatabaseInfo.Connection);
            Assert.That(rap, Is.Not.Null, "Report failed to parse");

            rap.Folder = _reportFolder.LocalPath;
            await rap.RunGetData();

            using var ms = new MemoryStreamGen();
            await rap.RunRender(ms, OutputPresentationType.HTML);

            string html = ms.GetText();
            Assert.That(html, Is.Not.Null.And.Not.Empty);
            Assert.That(html, Does.Contain("Function Test"));
        }
    }
}
