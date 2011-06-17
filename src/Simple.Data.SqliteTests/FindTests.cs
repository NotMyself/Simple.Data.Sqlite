using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using Simple.Data.Ado;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class FindTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        [TestFixtureSetUp]
        public void DeleteAlice()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            db.Employees.DeleteByFirstName("Dirk");
        }

        [Test]
        public void TestProviderWithFileName()
        {
            var provider = new ProviderHelper().GetProviderByFilename(DatabasePath);
            Assert.IsInstanceOf(typeof(SqliteConnectionProvider), provider);
        }

        [Test]
        public void TestProviderWithConnectionString()
        {
            var provider = new ProviderHelper().GetProviderByConnectionString(string.Format("data source={0}", DatabasePath));
            Assert.IsInstanceOf(typeof(SqliteConnectionProvider), provider);
        }

        [Test]
        public void TestFindByEmployeeID()
        {
            var db = Database.Opener.OpenFile(DatabasePath);
            var employee = db.Employees.FindByEmployeeID(1);
            Assert.AreEqual(1, employee.EmployeeID);
        }

        [Test]
        public void TestAll()
        {
            var db = Database.OpenFile(DatabasePath);
            var all = new List<object>(db.Employees.All().Cast<dynamic>());
            Assert.IsNotEmpty(all);
        }

        [Test]
        public void TestImplicitCast()
        {
            var db = Database.OpenFile(DatabasePath);
            Employee employee = db.Employees.FindByEmployeeID(1);
            Assert.AreEqual(1, employee.EmployeeID);
        }

        public class Employee
        {
            public long EmployeeID { get; set; }
        }

        [Test]
        public void TestImplicitEnumerableCast()
        {
            var db = Database.OpenFile(DatabasePath);
            foreach (Employee employee in db.Employees.All())
            {
                Assert.IsNotNull(employee);
            }
        }

        [Test]
        public void TestInsert()
        {
            var db = Database.OpenFile(DatabasePath);

            var employee = db.Employees.Insert(FirstName: "Dirk", LastName: "Diggler", BirthDate: DateTime.Today);

            Assert.IsNotNull(employee);
            Assert.AreEqual("Dirk", employee.FirstName);
            Assert.AreEqual("Diggler", employee.LastName);
            Assert.AreEqual(DateTime.Today, employee.BirthDate);
        }
    }
}