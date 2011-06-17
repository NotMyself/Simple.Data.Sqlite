using System.Data;
using NUnit.Framework;
using Simple.Data.Ado;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class InMemoryUsageTests
    {
        const string connectionString = "Data Source=:memory:";
        IDbConnection connection;
        dynamic db;

        [SetUp]
        public void SetUp()
        {
            var createTableSql = Properties.Resources.CreateEmployeesTable;
            db = Database.OpenConnection(connectionString);
            connection = ((AdoAdapter) db.GetAdapter()).ConnectionProvider.CreateConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            command.ExecuteNonQuery();
            
        }

        [TearDown]
        public void TearDown()
        {
            ((SqliteInMemoryDbConnection) connection).KillDashNine();
            connection = null;
        }

        [Test]
        public void CanCreateInMemoryDatabase()
        {
            Assert.Pass();
        }

        [Test]
        public void CanQueryInMemoryDatabase()
        {
            var employees = db.Employees.All();
            Assert.That(employees.ToList().Count, Is.EqualTo(3));
        }

        [Test]
        public void CanInsertInMemoryDatabase()
        {
            var employee = db.Employees.Insert(EmpName: "Dirk Diggler", EmpSalary: 100000);
            Assert.That(employee.EmpName, Is.EqualTo("Dirk Diggler"));
            Assert.That(employee.EmpSalary, Is.EqualTo(100000));
            Assert.That(employee.EmpID, Is.GreaterThan(0));
        }
    }
}