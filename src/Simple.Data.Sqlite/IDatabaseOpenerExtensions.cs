using System.Data;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    public static class IDatabaseOpenerExtensions
    {
        public static IInMemoryDbConnection OpenMemoryConnection(this IDatabaseOpener opener, string connectionString)
        {
            AdoAdapter adapter = opener.OpenConnection(connectionString).GetAdapter();
            return adapter.ConnectionProvider.CreateConnection() as IInMemoryDbConnection;
        }
    }
}
