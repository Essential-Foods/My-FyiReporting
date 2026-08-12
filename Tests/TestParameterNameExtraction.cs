using Majorsilence.Reporting.RdlDesign;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestParameterNameExtraction
    {
        [TestCase("=Parameters!Test.Value", "Test")]
        [TestCase("={?Test}", "Test")]
        public void ExtractNameFromParameterExpression(string expression, string expectedParameterName)
        {
            var result = DesignerUtility.ExtractParameterNameFromParameterExpression(expression);
            Assert.That(result, Is.EqualTo(expectedParameterName));
        }
    }
}
