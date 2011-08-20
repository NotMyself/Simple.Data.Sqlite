using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Simple.Data.Sqlite
{
    public interface IInMemoryDbConnection : IDbConnection
    {
        void Destroy();
    }
}
