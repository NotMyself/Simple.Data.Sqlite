using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    class Employee
    {
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    [TestFixture]
    public class InsertTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");
        
        [Test]
        public void TestInsertWithNamedArguments()
        {
            var db = Database.Opener.OpenFile(DatabasePath);

            var employee = db.Employees.Insert(FirstName: "Dirk", LastName: "Diggler", BirthDate: DateTime.Today);

            Assert.IsNotNull(employee);
            Assert.AreEqual("Dirk", employee.FirstName);
            Assert.AreEqual("Diggler", employee.LastName);
            Assert.AreEqual(DateTime.Today, employee.BirthDate);
        }

        [Test]
        public void TestInsertWithStaticTypeObject()
        {
            var db = Database.Opener.OpenFile(DatabasePath);

            var employee = new Employee { FirstName = "Jack", LastName = "Horner", BirthDate = DateTime.Today};

            var actual = db.Employees.Insert(employee);

            Assert.IsNotNull(employee);
            Assert.AreEqual("Jack", actual.FirstName);
            Assert.AreEqual("Horner", actual.LastName);
            Assert.AreEqual(DateTime.Today, actual.BirthDate);
        }

        [Test]
        public void TestMultiInsertWithStaticTypeObjects()
        {
            var db = Database.Opener.OpenFile(DatabasePath);

            var employees = new[]
                            {
                                new Employee { FirstName = "Jack", LastName = "Horner", BirthDate = DateTime.Today},
                                new Employee { FirstName = "Dirk", LastName = "Diggler", BirthDate = DateTime.Today}
                            };

            var actuals = db.Employees.Insert(employees).ToList<Employee>();

            Assert.AreEqual(2, actuals.Count);
            Assert.AreNotEqual(0, actuals[0].EmployeeId);
            Assert.AreEqual("Jack", actuals[0].FirstName);
            Assert.AreEqual("Horner", actuals[0].LastName);
            Assert.AreEqual(DateTime.Today, actuals[0].BirthDate);

            Assert.AreNotEqual(0, actuals[1].EmployeeId);
            Assert.AreEqual("Dirk", actuals[1].FirstName);
            Assert.AreEqual("Diggler", actuals[1].LastName);
            Assert.AreEqual(DateTime.Today, actuals[1].BirthDate);
        }

        [Test]
        public void TestInsertWithDynamicTypeObject()
        {
            var db = Database.Opener.OpenFile(DatabasePath);

            dynamic employee = new ExpandoObject();
            employee.FirstName = "Dirk";
            employee.LastName = "Diggler";
            employee.BirthDate = DateTime.Today;

            var actual = db.Employees.Insert(employee);

            Assert.IsNotNull(employee);
            Assert.AreEqual("Dirk", actual.FirstName);
            Assert.AreEqual("Diggler", actual.LastName);
            Assert.AreEqual(DateTime.Today, actual.BirthDate);
        }

        [Test]
        public void TestMultiInsertWithDynamicTypeObjects()
        {
            var db = Database.Opener.OpenFile(DatabasePath);

            dynamic employee1 = new ExpandoObject();
            employee1.FirstName = "Dirk";
            employee1.LastName = "Diggler";
            employee1.BirthDate = DateTime.Today;

            dynamic employee2 = new ExpandoObject();
            employee2.FirstName = "Jack";
            employee2.LastName = "Horner";
            employee2.BirthDate = DateTime.Today;

            var users = new[] { employee1, employee2 };

            IList<dynamic> actuals = db.Employees.Insert(users).ToList();

            Assert.AreEqual(2, actuals.Count);
            Assert.AreNotEqual(0, actuals[0].EmployeeId);
            Assert.AreEqual("Dirk", actuals[0].FirstName);
            Assert.AreEqual("Diggler", actuals[0].LastName);
            Assert.AreEqual(DateTime.Today, actuals[0].BirthDate);

            Assert.AreNotEqual(0, actuals[1].EmployeeId);
            Assert.AreEqual("Jack", actuals[1].FirstName);
            Assert.AreEqual("Horner", actuals[1].LastName);
            Assert.AreEqual(DateTime.Today, actuals[1].BirthDate);
        }
    }
}
