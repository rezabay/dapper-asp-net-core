namespace Dapper.AspNetCore.Query
{
	public class SqlQuery<T> : SqlQueryBase
    {
        public bool IsReadOnly { get; protected set; } = true;

		protected SqlQuery(string sql): base(sql)
        {
        }
    }
}
