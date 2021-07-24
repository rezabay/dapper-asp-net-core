using System;
using System.Data;

namespace Dapper.AspNetCore.Query
{
	public class SqlQueryBase
    {
        public string Sql { get; }
        public object Parameters { get; protected set; }
        public CommandType Type { get; protected set; }

        protected SqlQueryBase(string sql)
        {
            Sql = sql;
        }
    }
}
