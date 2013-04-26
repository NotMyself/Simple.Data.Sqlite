using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    [Export(typeof(IQueryPager))]
    public class SqliteQueryPager : IQueryPager
    {
        private static readonly Regex ColumnExtract = new Regex(@"SELECT\s*(.*)\s*(FROM.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public IEnumerable<string> ApplyLimit(string sql, int take)
        {
            sql = AddMissingOrderBy(sql);
            yield return string.Format("{0} LIMIT {1}", sql, take);
        }

        public IEnumerable<string> ApplyPaging(string sql, string[] keys, int skip, int take)
        {
            sql = AddMissingOrderBy(sql, keys);

            yield return string.Format("{0} LIMIT {1},{2}", sql, skip, take);
        }

        private string AddMissingOrderBy(string sql, string[] keys = null)
        {
            if (sql.IndexOf("order by", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                if (keys != null && keys.Length > 0)
                {
                    sql += " ORDER BY " + string.Join(", ", keys);
                }
                else
                {
                    var match = ColumnExtract.Match(sql);
                    var columns = match.Groups[1].Value.Trim().Replace(" AS ", ",").Split(',');

                    sql += " ORDER BY " + columns.First().Trim();
                }
            }
            return sql;
        }
    }
}