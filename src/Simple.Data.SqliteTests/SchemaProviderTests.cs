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
            var table = schemaProvider.GetTables().FirstOrDefault(t => t.ActualName == "Products");
            Assert.IsNotNull(table);

            var foreignKeys = schemaProvider.GetForeignKeys(table);
            Assert.IsNotNull(foreignKeys);
            Assert.IsTrue(foreignKeys.Count() == 2);
        }
    }
}
