using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace Dapper.AspNetCore
{
	internal static class TransientFaultHandling
	{
		public static bool IsTransient(Exception ex)
		{
			if (ex == null)
			{
				return false;
			}

			SqlException sqlException;
			if ((sqlException = ex as SqlException) != null)
			{
				// Enumerate through all errors found in the exception.
				foreach (SqlError err in sqlException.Errors)
				{
					switch (err.Number)
					{
						// The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
						case 40501:

						// Resource ID: %d. The %s limit for the database is %d and has been reached.
						case 10928:

						// Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. 
						// However, the server is currently too busy to support requests greater than %d for this database.
						case 10929:

						// A transport-level error has occurred when receiving results from the server.
						// An established connection was aborted by the software in your host machine.
						case 10053:

						// A transport-level error has occurred when sending the request to the server. 
						// (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
						case 10054:

						// A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
						// The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
						// is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed 
						// because the connected party did not properly respond after a period of time, or established connection failed 
						// because connected host has failed to respond.)"}
						case 10060:

						// Login failed for user '%s'. Reason: Server is in script upgrade mode. Only administrator can connect at this time.
						// Devnote: this can happen when SQL is going through recovery (e.g. after failover)
						case 18401:

						// The service has encountered an error processing your request. Please try again.
						case 40197:

						// The service has encountered an error processing your request. Please try again.
						case 40540:

						// Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer 
						// support, and provide them the session tracing ID of ZZZZZ.
						case 40613:

						// The service has encountered an error processing your request. Please try again.
						case 40143:

						// The client was unable to establish a connection because of an error during connection initialization process before login. 
						// Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy 
						// to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. 
						// (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
						case 233:

						// A connection was successfully established with the server, but then an error occurred during the login process. 
						// (provider: TCP Provider, error: 0 - The specified network name is no longer available.) 
						case 64:

						// The instance of SQL Server you attempted to connect to does not support encryption.
						case 20:
							return true;
					}
				}

				// Prelogin failure can happen due to waits expiring on windows handles. Or
				// due to a bug in the gateway code, a dropped database with a pooled connection
				// when reset results in a timeout error instead of immediate failure.
				if (sqlException.InnerException is Win32Exception wex)
				{
					switch (wex.NativeErrorCode)
					{
						// Timeout expired
						case 0x102:
							return true;

						// Semaphore timeout expired
						case 0x121:
							return true;
					}
				}
			}
			else if (ex is TimeoutException)
			{
				return true;
			}

			return false;
		}
	}
}
