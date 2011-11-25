using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Simple.Data.Sqlite;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class QueryPagerTests
    {
        static readonly Regex Normalize = new Regex(@"\s+", RegexOptions.Multiline);

        [Test]
        public void ShouldApplyPagingUsingOrderBy()
        {
            const string sql = "select a,b,c from d where a = 1 order by c";
            const string expected =
                "select a,b,c from d where a = 1 order by c limit 5,10";

            var modified = new SqliteQueryPager().ApplyPaging(sql, 5, 10).Single();
            modified = Normalize.Replace(modified, " ").ToLowerInvariant();

            Assert.AreEqual(expected, modified);
        }

        [Test]
        public void ShouldApplyPagingUsingOrderByFirstColumnIfNotAlreadyOrdered()
        {
            const string sql = "select a,b,c from d where a = 1";
            const string expected =
                "select a,b,c from d where a = 1 order by a limit 5,10";

            var modified = new SqliteQueryPager().ApplyPaging(sql, 5, 10).Single();
            modified = Normalize.Replace(modified, " ").ToLowerInvariant();

            Assert.AreEqual(expected, modified);
        }

    }
}