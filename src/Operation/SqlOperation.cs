using Dapper.AspNetCore.Query;

namespace Dapper.AspNetCore.Operation
{
	public class SqlOperation : SqlQueryBase
    {
        protected SqlOperation(string sql): base(sql)
        {
        }
    }
}
