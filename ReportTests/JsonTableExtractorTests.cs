using System;
using System.Collections.Generic;
using System.Data;
using Majorsilence.Reporting.Data;
using NUnit.Framework;

namespace ReportTests
{
    [TestFixture]
    public class JsonTableExtractorTests
    {
        private static Dictionary<string, IDataReader> Extract(string json) =>
            new JsonTableExtractor().Extract(json);

        // ── Flat array root ───────────────────────────────────────────────────────

        [Test]
        public void FlatArray_ProducesRootTableWithAllColumns()
        {
            var tables = Extract("""
                [{"Name":"Alice","Age":30},{"Name":"Bob","Age":25}]
                """);

            Assert.That(tables.ContainsKey("root"), "Expected 'root' table");
            var reader = tables["root"];

            Assert.That(reader.Read());
            Assert.That(reader["Name"].ToString(), Is.EqualTo("Alice"));
            Assert.That(reader["Age"], Is.EqualTo(30L));

            Assert.That(reader.Read());
            Assert.That(reader["Name"].ToString(), Is.EqualTo("Bob"));
        }

        [Test]
        public void FlatArray_RowCount_MatchesInputLength()
        {
            var tables = Extract("""
                [{"x":1},{"x":2},{"x":3}]
                """);
            var reader = tables["root"];
            int count = 0;
            while (reader.Read()) count++;
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void FlatArray_EmptyArray_ProducesRootTableWithZeroRows()
        {
            var tables = Extract("[]");
            var reader = tables["root"];
            Assert.That(reader.Read(), Is.False, "Empty array should yield no rows");
        }

        // ── Object root ───────────────────────────────────────────────────────────

        [Test]
        public void ObjectRoot_ProducesSingleRowInRootTable()
        {
            var tables = Extract("""
                {"Name":"Alice","Score":99}
                """);

            Assert.That(tables.ContainsKey("root"), "Expected 'root' table");
            var reader = tables["root"];

            Assert.That(reader.Read());
            Assert.That(reader["Name"].ToString(), Is.EqualTo("Alice"));
            Assert.That(reader["Score"], Is.EqualTo(99L));
            Assert.That(reader.Read(), Is.False, "Object root should produce exactly one row");
        }

        // ── Nested objects (dot-notation flattening) ──────────────────────────────

        [Test]
        public void NestedObject_FlattensPropertiesWithUnderscorePrefix()
        {
            var tables = Extract("""
                [{"Name":"Alice","Address":{"City":"Toronto","Zip":"M5V"}}]
                """);

            var reader = tables["root"];
            Assert.That(reader.Read());

            // Flattened scalar properties of the nested object are prefixed
            Assert.That(reader["Address_City"].ToString(), Is.EqualTo("Toronto"));
            Assert.That(reader["Address_Zip"].ToString(), Is.EqualTo("M5V"));
        }

        // ── Array children (child tables with __parent_guid) ─────────────────────

        [Test]
        public void ArrayChild_ProducesChildTable()
        {
            var tables = Extract("""
                [{"Name":"Alice","Orders":[{"OrderID":1},{"OrderID":2}]}]
                """);

            Assert.That(tables.ContainsKey("root_Orders"), "Expected 'root_Orders' child table");
            var childReader = tables["root_Orders"];
            int childCount = 0;
            while (childReader.Read()) childCount++;
            Assert.That(childCount, Is.EqualTo(2));
        }

        [Test]
        public void ArrayChild_HasParentGuidColumn()
        {
            var tables = Extract("""
                [{"ID":1,"Tags":["a","b"]}]
                """);

            Assert.That(tables.ContainsKey("root_Tags"), "Expected 'root_Tags' child table");
            var childReader = tables["root_Tags"];
            Assert.That(childReader.Read());
            // __parent_guid links child rows to the parent row
            Assert.That(childReader["__parent_guid"], Is.Not.Null);
        }

        // ── Primitive types ───────────────────────────────────────────────────────

        [Test]
        public void PrimitiveTypes_ArePreservedCorrectly()
        {
            var tables = Extract("""
                [{"IntVal":42,"DoubleVal":3.14,"BoolTrue":true,"BoolFalse":false,"NullVal":null,"StrVal":"hello"}]
                """);

            var reader = tables["root"];
            Assert.That(reader.Read());

            Assert.That(reader["IntVal"], Is.EqualTo(42L));
            Assert.That(reader["DoubleVal"], Is.EqualTo(3.14));
            Assert.That(reader["BoolTrue"], Is.True);
            Assert.That(reader["BoolFalse"], Is.False);
            Assert.That(reader["NullVal"], Is.EqualTo(DBNull.Value), "JSON null should map to DBNull.Value per ADO.NET convention");
            Assert.That(reader["StrVal"].ToString(), Is.EqualTo("hello"));
        }

        [Test]
        public void LargeInteger_StoredAsLong()
        {
            var tables = Extract("""[{"BigNum":9876543210}]""");
            var reader = tables["root"];
            Assert.That(reader.Read());
            Assert.That(reader["BigNum"], Is.EqualTo(9876543210L));
        }

        // ── Multiple rows, mixed data ─────────────────────────────────────────────

        [Test]
        public void MultipleRows_AllRowsReturnedInOrder()
        {
            var tables = Extract("""
                [{"ID":1,"Name":"Alpha"},{"ID":2,"Name":"Beta"},{"ID":3,"Name":"Gamma"}]
                """);

            var reader = tables["root"];
            var names = new List<string>();
            while (reader.Read())
                names.Add(reader["Name"].ToString());

            Assert.That(names, Is.EqualTo(new[] { "Alpha", "Beta", "Gamma" }));
        }

        // ── __guid column ─────────────────────────────────────────────────────────

        [Test]
        public void RootRows_HaveGuidColumn()
        {
            var tables = Extract("""[{"X":1},{"X":2}]""");
            var reader = tables["root"];

            var guids = new HashSet<string>();
            while (reader.Read())
            {
                string guid = reader["__guid"].ToString();
                Assert.That(guid, Is.Not.Null.And.Not.Empty);
                guids.Add(guid);
            }
            Assert.That(guids.Count, Is.EqualTo(2), "Each row should have a unique __guid");
        }
    }
}
