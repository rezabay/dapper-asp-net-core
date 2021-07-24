using System.Data;

namespace Dapper.AspNetCore.Factories
{
	public interface IConnectionFactory
    {
        IDbConnection CreateReadOnlyConnection();
        IDbConnection CreateReadWriteConnection();
    }
}