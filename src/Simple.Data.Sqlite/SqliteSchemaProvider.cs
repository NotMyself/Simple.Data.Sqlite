using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Sqlite
{
    public class SqliteSchemaProvider : ISchemaProvider
    {
        readonly IConnectionProvider _connectionProvider;

        public SqliteSchemaProvider(SqliteConnectionProvider connectionProvider)
        {
            if (connectionProvider == null) throw new ArgumentNullException("connectionProvider");
            _connectionProvider = connectionProvider;
        }

        public IConnectionProvider ConnectionProvider
        {
            get { return _connectionProvider; }
        }

        public IEnumerable<Table> GetTables()
        {
            return GetSchema("TABLES").Select(SchemaRowToTable);
        }

        private static Table SchemaRowToTable(DataRow row)
        {
            return new Table(row["TABLE_NAME"].ToString(), row["TABLE_SCHEMA"].ToString(),
                             row["TABLE_TYPE"].ToString() == "BASE TABLE" ? TableType.Table : TableType.View);
        }

        private IEnumerable<DataRow> GetSchema(string collectionName, params string[] constraints)
        {
            using (var cn = ConnectionProvider.CreateConnection())
            {
                cn.Open();

                return cn.GetSchema(collectionName, constraints).AsEnumerable();
            }
        }

        public IEnumerable<Column> GetColumns(Table table)
        {
            return GetSchema( "COLUMNS", new[] { null, null, table.ActualName } )
                .AsEnumerable()
                .Select( row => new Column( row.Field<string>( "COLUMN_NAME" ), table, row.Field<bool>( "AUTOINCREMENT" ) ) );
        }

        public IEnumerable<Procedure> GetStoredProcedures()
        {
            return Enumerable.Empty<Procedure>();
        }

        public IEnumerable<Parameter> GetParameters(Procedure storedProcedure)
        {
            return Enumerable.Empty<Parameter>();
        }

        public Key GetPrimaryKey(Table table)
        {
            return new Key( GetSchema( "COLUMNS", new[] { null, null, table.ActualName } )
                .AsEnumerable()
                .Where( row => row.Field<bool>( "PRIMARY_KEY" ) )
                .Select( row => row.Field<string>( "COLUMN_NAME" ) ) );
        }

        public IEnumerable<ForeignKey> GetForeignKeys(Table table)
        {
            var groups = GetSchema( "FOREIGNKEYS", new[] { null, null, table.ActualName } )
                .AsEnumerable()
                .GroupBy( row => new
                {
                    CatalogName = row.Field<string>( "FKEY_TO_CATALOG" ),
                    SchemaName = row.Field<string>( "FKEY_TO_SCHEMA" ),
                    TableName = row.Field<string>( "FKEY_TO_TABLE" )
                } );

            foreach (var group in groups)
            {
                var masterName = new ObjectName( null, group.Key.TableName );
                var detailName = new ObjectName(null, table.ActualName);
                var key = new ForeignKey(
                    detailName, 
                    group.Select(row => row.Field<string>("FKEY_TO_COLUMN")), 
                    masterName,
                    group.Select(row => row.Field<string>("FKEY_FROM_COLUMN")));
                yield return key;
            }
        }

        public string QuoteObjectName(string unquotedName)
        {
            if (unquotedName == null) throw new ArgumentNullException("unquotedName");
            if (unquotedName.StartsWith("[")) return unquotedName;
            return string.Concat("[", unquotedName, "]");
        }

        public string NameParameter(string baseName)
        {
            if (baseName == null) throw new ArgumentNullException("baseName");
            if (baseName.Length == 0) throw new ArgumentException("Base name must be provided");
            return (baseName.StartsWith("@")) ? baseName : "@" + baseName;
        }

        public string GetDefaultSchema()
        {
            return String.Empty;
        }

        private DataTable GetColumnsDataTable(Table table)
        {
            return SelectToDataTable("pragma table_info(" + table.ActualName + ");");
        }

        private DataTable SelectToDataTable(string sql)
        {
            var dataTable = new DataTable();
            using (var cn = ConnectionProvider.CreateConnection())
            {
                //TODO:this is hacky has hell but the data adapter requires the concrete type.
                var con = cn.CreateCommand().Connection as SQLiteConnection;
                using (var adapter = new SQLiteDataAdapter(sql, con))
                {
                    adapter.Fill(dataTable);
                }

            }

            return dataTable;
        }
    }
}