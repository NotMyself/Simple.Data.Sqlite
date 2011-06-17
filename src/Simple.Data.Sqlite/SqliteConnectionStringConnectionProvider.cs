using System.ComponentModel.Composition;
using System.Data;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    [Export("sql", typeof(IConnectionProvider))]
    public class SqliteConnectionStringConnectionProvider : SqliteConnectionProvider
    {
        IDbConnection _connection;

        public override IDbConnection CreateConnection()
        {
            if (ConnectionString.Contains(":memory:"))
            {
                if (_connection == null)
                {
                    return _connection = new SqliteInMemoryDbConnection(base.CreateConnection());
                }
                return _connection;
            }
            return base.CreateConnection();
        }
    }
}