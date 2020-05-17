namespace Passingwind.Blog.Data
{
	public class DbConnectionString
	{
		public DbConnectionProvider Provider { get; set; }
		public string ConnectionString { get; set; }
	}

	public enum DbConnectionProvider
	{ 
		SqlServer,
		Sqlite,
		Mysql,
		PostgreSQL,
	}
}
