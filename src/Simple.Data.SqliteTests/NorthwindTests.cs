using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class NorthwindTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        [Test]
        public void LikeQueryShouldRunAfterAnotherQuery()
        {
            var db = Database.OpenFile(DatabasePath);
            var products = db.Products.FindAll(db.Products.CategoryId == 4);
            var data = db.Products.FindAll(db.Products.ProductName.Like("C%"));
            Assert.Pass();
        }

        [Test]
        public void QueryShouldRunAfterALikeQuery()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Products.FindAll(db.Products.ProductName.Like("C%"));
            var products = db.Products.FindAll(db.Products.CategoryId == 4);
            Assert.Pass();
        }
    }
}