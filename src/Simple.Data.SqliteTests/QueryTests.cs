using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class QueryTests
    {
        private static readonly string DatabasePath = Path.Combine(
           Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
           "Northwind.db");

        [TestFixtureSetUp]
        public void InsertRecords()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            
            foreach (var number in Enumerable.Range(2,100))
            {
                var item = db.PagingTest.Insert(row: number);
                Console.WriteLine(item.Id);
            }
        }


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
