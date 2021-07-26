namespace Dapper.AspNetCore.Query
{
	public abstract class SqlQueryJson<T> : SqlQuery<string>
	{
		protected SqlQueryJson(string sql) : base(sql)
		{
		}
	}
}
