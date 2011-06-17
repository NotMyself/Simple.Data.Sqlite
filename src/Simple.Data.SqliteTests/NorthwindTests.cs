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
            var item = data.First();
            Assert.Pass();
        }

        [Test]
        public void QueryShouldRunAfterALikeQuery()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Products.FindAll(db.Products.ProductName.Like("C%"));
            var products = db.Products.FindAll(db.Products.CategoryId == 4);
            var product = products.First();
            Assert.Pass();
        }

        [Test]
        public void CanOrderBySomeField()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().OrderByOrderDate();
            var order = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanOrderBySomeFieldDecending()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().OrderByOrderDateDescending();
            var order = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanOrderByTwoFields()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().OrderByOrderDate().ThenByFreight();
            var order = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanPageQueries()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().Skip(5).Take(5);
            var order = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanExplicitlySelectColumns()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().Select(db.Orders.OrderDate,db.Orders.Freight);
            var order = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanAliasSomeField()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().Select(db.Orders.OrderDate, db.Orders.Freight.As("Poo"));
            var order = data.First();
            Assert.That(order.Poo,Is.Not.Null);
        }

        [Test]
        [Ignore]
        public void CanAggregateATable()
        {
            var db = Database.OpenFile(DatabasePath);
            long data = db.Orders.All().Count();
            Assert.That(data,Is.GreaterThan(0));
        }

        [Test]
        public void CanAggregateAColumn()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.Query().Select(db.Orders.OrderDate, db.Orders.Freight.Average().As("AverageFreight"));
            var item = data.First();
            Assert.Pass();
        }

        [Test]
        public void CanGetLenthOfAField()
        {
            var db = Database.OpenFile(DatabasePath);
            var data = db.Orders.All().Select(db.Orders.ShipName.As("NameLength"));
            var item = data.First();
            Assert.Pass();
        }
    }
}