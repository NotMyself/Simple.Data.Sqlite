using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Simple.Data.TestHelper;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class DatabaseSchemaTests : DatabaseSchemaTestsBase
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        protected override Database GetDatabase()
        {
            return Database.OpenFile(DatabasePath);
        }

        [Test]
        public void TestTables()
        {
            Assert.AreEqual(1, Schema.Tables.Count(t => t.ActualName == "Customers"));
        }

        [Test]
        public void TestColumns()
        {
            var table = Schema.FindTable("Customers");
            Assert.AreEqual(1, table.Columns.Count(c => c.ActualName == "CustomerID"));
        }

        [Test]
        public void TestPrimaryKey()
        {
            Assert.AreEqual("CategoryID", Schema.FindTable("Categories").PrimaryKey[0]);
        }

        [Test]
        [Ignore]
        public void TestForeignKey()
        {
            var foreignKey = Schema.FindTable("Orders").ForeignKeys.Single();
            Assert.AreEqual("Customers", foreignKey.MasterTable);
            Assert.AreEqual("Orders", foreignKey.DetailTable);
            Assert.AreEqual("CustomerId", foreignKey.Columns[0]);
            Assert.AreEqual("CustomerId", foreignKey.UniqueColumns[0]);
        }

        [Test]
        public void TestSingularResolution()
        {
            Assert.AreEqual("Products",
                Schema.FindTable("Product").ActualName);
        }
    }
}
