using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    //TODO: add a table to the northwind db to make these tests pass.
    [TestFixture]
    [Ignore]
    public class QueryTests
    {
        private static readonly string DatabasePath = Path.Combine(
           Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
           "Northwind.db");

        [Test]
        public void ShouldSelectFromOneToTen()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            var query = db.PagingTest.QueryById(1.to(100)).Take(10);
            int index = 1;
            foreach (var row in query)
            {
                Assert.AreEqual(index, row.Id);
                index++;
            }
        }

        [Test]
        public void ShouldSelectFromElevenToTwenty()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            var query = db.PagingTest.QueryById(1.to(100)).Skip(10).Take(10);
            int index = 11;
            foreach (var row in query)
            {
                Assert.AreEqual(index, row.Id);
                index++;
            }
        }

        [Test]
        public void ShouldSelectFromOneHundredToNinetyOne()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            var query = db.PagingTest.QueryById(1.to(100)).OrderByDescending(db.PagingTest.Id).Skip(0).Take(10);
            int index = 100;
            foreach (var row in query)
            {
                Assert.AreEqual(index, row.Id);
                index--;
            }
        }
    }
}
