using System.Threading.Tasks;
using Dapper.AspNetCore.Operation;
using Dapper.AspNetCore.Query;

namespace Dapper.AspNetCore.Repository
{
	public interface IAppRepository
	{
		Task<T> QueryFirstOrDefaultAsync<T>(SqlQuery<T> query);
		Task<T[]> QueryAsync<T>(SqlQuery<T> query);
		Task<T> QueryJsonAsync<T>(SqlQueryJson<T> query);
		Task<int> ExecuteAsync(SqlOperation operation);
	}
}
