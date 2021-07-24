using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Dapper.AspNetCore.Factories
{
	public class ConnectionFactory : IConnectionFactory
    {
        private readonly DapperOptions _options;

        public ConnectionFactory(IOptions<DapperOptions> edmOptions)
        {
            _options = edmOptions.Value;
        }

        public IDbConnection CreateReadOnlyConnection()
        {
            return new SqlConnection(_options.ReadOnlyConnectionString);
        }

        public IDbConnection CreateReadWriteConnection()
        {
            return new SqlConnection(_options.ReadWriteConnectionString);
        }
    }
}
