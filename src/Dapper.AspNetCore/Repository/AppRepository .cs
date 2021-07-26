using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper.AspNetCore.Factories;
using Dapper.AspNetCore.Helpers;
using Dapper.AspNetCore.Operation;
using Dapper.AspNetCore.Query;
using Microsoft.Extensions.Options;

namespace Dapper.AspNetCore.Repository
{
	public class AppRepository : IAppRepository
	{
		private readonly IConnectionFactory _connectionFactory;
		private readonly DapperOptions _options;

		public AppRepository(IConnectionFactory connectionFactory, IOptions<DapperOptions> options)
		{
			_connectionFactory = connectionFactory;
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<T[]> QueryAsync<T>(SqlQuery<T> query)
		{
			return ExecuteWithRetry(async () =>
			{
				using var dbConnection = CreateConnection(query.IsReadOnly);
				var results = await dbConnection.QueryAsync<T>(query.Sql,
					query.Parameters,
					commandType: query.Type);

				return results.ToArray();
			});
		}

		public Task<T> QueryFirstOrDefaultAsync<T>(SqlQuery<T> query)
		{
			return ExecuteWithRetry(async () =>
			{
				using var dbConnection = CreateConnection(query.IsReadOnly);
				var results = await dbConnection.QueryFirstOrDefaultAsync<T>(query.Sql,
					query.Parameters,
					commandType: query.Type);

				return results;
			});
		}

		public Task<T> QueryJsonAsync<T>(SqlQueryJson<T> query)
		{
			return ExecuteWithRetry(async () =>
			{
				using var dbConnection = CreateConnection(query.IsReadOnly);
				var result = await dbConnection.QuerySingleOrDefaultAsync<string>(query.Sql,
					query.Parameters,
					commandType: query.Type);
				return JsonHelper.Deserialize<T>(result);
			});
		}

		public Task<int> ExecuteAsync(SqlOperation operation)
		{
			return ExecuteWithRetry(async () =>
			{
				using var dbConnection = CreateConnection(readOnly: false);
				var result = await dbConnection.ExecuteAsync(operation.Sql,
					operation.Parameters,
					commandType: operation.Type);
				return result;
			});
		}

		private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action)
		{
			var numberOfRetries = Math.Max(1, _options.NumOfRetries);

			for (var i = 1; i <= numberOfRetries; i++)
			{
				try
				{
					var result = await action();
					return result;
				}
				catch (Exception ex)
				{
					if (i == numberOfRetries || !TransientFaultHandling.IsTransient(ex))
					{
						throw;
					}
				}

				Thread.Sleep(500);
			}

			return default;
		}

		private IDbConnection CreateConnection(bool readOnly)
		{
			return readOnly
				? _connectionFactory.CreateReadOnlyConnection()
				: _connectionFactory.CreateReadWriteConnection();
		}
	}
}
