using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class SchemaProviderTest
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        SqliteSchemaProvider schemaProvider;

        [SetUp]
        public void Setup()
        {
            var connectionProvider = new SqliteConnectionProvider();
            connectionProvider.SetConnectionString(string.Format("Data Source={0}", DatabasePath));
            schemaProvider = new SqliteSchemaProvider(connectionProvider);
        }

        [Test]
        public void NullConnectionProviderCausesException()
        {
            Assert.Throws<ArgumentNullException>(() => new SqliteSchemaProvider(null));
        }

        [Test]
        public void ProceduresIsEmpty()
        {
            Assert.AreEqual(0, new SqliteSchemaProvider(new SqliteConnectionProvider()).GetStoredProcedures().Count());
        }

        [Test]
        public void TestPrimaryKeys()
        {
            var productTable = schemaProvider.GetTables().FirstOrDefault(t => t.ActualName == "Products");
            Assert.IsNotNull(productTable);

            var pks = schemaProvider.GetPrimaryKey(productTable);
            Assert.IsTrue(pks.Length == 1);
            Assert.AreEqual(pks[0], "ProductID");
        }

        [Test]
        public void TestDefaultSchema()
        {
            var defaultSchema = schemaProvider.GetDefaultSchema();

            Assert.AreEqual(String.Empty, defaultSchema);
        }

        [Test]
        public void TestIdentityColumn()
        {
            var productTable = schemaProvider.GetTables().FirstOrDefault(t => t.ActualName == "Products");
            Assert.IsNotNull(productTable);

            var idColumn = schemaProvider.GetColumns(productTable).FirstOrDefault(c => c.ActualName == "ProductID");
            Assert.IsNotNull(idColumn);
            Assert.IsTrue(idColumn.IsIdentity);
        }

        [Test]
        public void TestNotIdentityColumn()
        {
            var table = schemaProvider.GetTables().FirstOrDefault(t => t.ActualName == "InternationalOrders");
            Assert.IsNotNull(table);

            var idColoumn = schemaProvider.GetColumns(table).FirstOrDefault(c => c.ActualName == "OrderID");
            Assert.IsNotNull(idColoumn);
            Assert.IsFalse(idColoumn.IsIdentity);
        }

        [Test]
        public void TestForeignKeys()
        {
            var table = schemaProvider.GetTables().FirstOrDefault(t => t.ActualName == "ForeignKeyTest");
            Assert.IsNotNull(table);

            var foreignKeys = schemaProvider.GetForeignKeys(table);
            Assert.IsNotNull(foreignKeys);
            Assert.IsTrue(foreignKeys.Count() == 3);
            foreach (var key in foreignKeys)
            {
                StringAssert.AreEqualIgnoringCase("ForeignKeyTest", key.DetailTable.Name);
                Assert.AreEqual(1, key.Columns.Length);
                Assert.AreEqual(1, key.UniqueColumns.Length);
                switch (key.MasterTable.Name)
                {
                    case "ForeignKeyTest":
                        StringAssert.AreEqualIgnoringCase("Id", key.UniqueColumns[0]);
                        StringAssert.AreEqualIgnoringCase("Parent", key.Columns[0]);
                        break;
                    case "Products":
                        StringAssert.AreEqualIgnoringCase("ProductID", key.UniqueColumns[0]);
                        StringAssert.AreEqualIgnoringCase("Product", key.Columns[0]);
                        break;
                    case "Regions":
                        StringAssert.AreEqualIgnoringCase("RegionID", key.UniqueColumns[0]);
                        StringAssert.AreEqualIgnoringCase("RegionID", key.Columns[0]);
                        break;
                    default:
                        Assert.Fail("Unexpected foreign key constraint", key.MasterTable.Name);
                        break;
                }
            }
        }
    }
}
