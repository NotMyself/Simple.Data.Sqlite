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
        IConnectionProvider _connectionProvider;

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
            //return new Key(GetColumns(table).Where(column => column.IsIdentity).Select(x => x.ActualName));
        }

        public IEnumerable<ForeignKey> GetForeignKeys(Table table)
        {
            var fkTest = GetSchema( "FOREIGNKEYS", new[] { null, null, table.ActualName } );
            foreach( var row in fkTest )
            {
                System.Diagnostics.Debug.WriteLine( "" );
            }

            var groups = SelectToDataTable("pragma foreign_key_list(" + table.ActualName + ");")
                .AsEnumerable()
                .GroupBy(row => row.Field<string>("table"));

            foreach (var group in groups)
            {
                var masterName = new ObjectName(null, group.First().Field<string>("table"));
                var detailName = new ObjectName(null, table.ActualName);
                var key = new ForeignKey(detailName, group.Select(row => row.Field<string>("to")), masterName,
                                         group.Select(row => row.Field<string>("from")));
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