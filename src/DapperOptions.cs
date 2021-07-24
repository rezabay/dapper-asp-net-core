namespace Dapper.AspNetCore
{
	public class DapperOptions
    {
        public string ReadOnlyConnectionString { get; set; }
        public string ReadWriteConnectionString { get; set; }
		public int NumOfRetries { get; set; } = 3;
	}
}
