using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class SchemaProviderTest
    {
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
    }
}
