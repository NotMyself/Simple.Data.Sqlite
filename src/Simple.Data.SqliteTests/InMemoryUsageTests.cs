using NUnit.Framework;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class InMemoryUsageTests
    {
        const string connectionString = "Data Source=:memory:";
        IInMemoryDbConnection connection;
        dynamic db;

        [SetUp]
        public void SetUp()
        {
            connection = Database.Opener.OpenMemoryConnection(connectionString);
            db = Database.OpenConnection(connectionString);
            PrepareDatabase();
            
        }

        [TearDown]
        public void TearDown()
        {
            connection.Destroy();
        }

        private void PrepareDatabase()
        {
            var createTableSql = Properties.Resources.CreateEmployeesTable;
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            command.ExecuteNonQuery();

        }
        
        [Test]
        public void CanCreateInMemoryDatabase()
        {
            Assert.Pass();
        }

        [Test, Ignore("sqlite bug")]
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