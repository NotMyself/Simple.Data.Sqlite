using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SQLite;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Sqlite
{
    [Export("db", typeof(IConnectionProvider))]
    [Export("sqlite", typeof(IConnectionProvider))]
    public class SqliteConnectionProvider : IConnectionProvider
    {
        string _connectionString;

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual IDbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public ISchemaProvider GetSchemaProvider()
        {
            return new SqliteSchemaProvider(this);
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public bool SupportsCompoundStatements
        {
            get { return true; }
        }

        public string GetIdentityFunction()
        {
            return "last_insert_rowid()";
        }

        public bool SupportsStoredProcedures
        {
            get { return false; }
        }

        public IProcedureExecutor GetProcedureExecutor(AdoAdapter adapter, ObjectName procedureName)
        {
            throw new NotSupportedException("Sqlite does not support stored procedures.");
        }
    }
}