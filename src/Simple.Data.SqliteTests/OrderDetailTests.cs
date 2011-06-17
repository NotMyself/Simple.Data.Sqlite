using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class OrderDetailTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        [Test]
        public void TestOrderDetail()
        {
            var db = Database.OpenFile(DatabasePath);
            var order = db.Orders.FindByOrderDate(new DateTime(1996, 07, 10));
            Assert.IsNotNull(order);
            var orderDetail = order.OrderDetails.FirstOrDefault();
            Assert.IsNotNull(orderDetail);
            var product = orderDetail.Product;
            Assert.AreEqual("Gorgonzola Telino", product.ProductName);
        }
    }
}