using System.Data;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    public class SqliteInMemoryDbConnection : DelegatingConnectionBase
    {
        public SqliteInMemoryDbConnection(IDbConnection target)
            : base(target)
        { }

        public override void Open()
        {
            if (DelegatedConnection.State == ConnectionState.Closed)
                DelegatedConnection.Open();
        }

        public override void Close()
        {
            //do not close explicitly
        }

        public override DataTable GetSchema(string collectionName, params string[] constraints)
        {
            return DelegatedConnection.GetSchema(collectionName, constraints);
        }

        public override void Dispose()
        {
            //do not dispose anything...
        }

        public void KillDashNine()
        {
            if (DelegatedConnection.State != ConnectionState.Closed)
                DelegatedConnection.Close();
        }

    }
}