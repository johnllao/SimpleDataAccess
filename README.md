Simple Data Access
================

Simple data access library for .NET

Examples
1. Using the call Execute(string sql, params IParameter[] parameters)
using (var db = new Database("MyConnectionStringName"))
{
	db.Execute("insert into Member (FirstName, LastName) values (@firstName, @lastName)",
		new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
		new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));
}

2. Using the call Query<T>(string sql)
using (var db = new Database("MyConnectionStringName"))
{
	var list = db.Query<Member>("select Id, FirstName, LastName from Member");
	foreach (var item in list)
	{
		Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
	}
}

3. Using the call Query<T>(string sql, params IParameter[] parameters)
using (var db = new Database("MyConnectionStringName"))
{
	var list = db.Query<Member>("select Id, FirstName, LastName from Member where Id=@Id", new Parameter("@Id", SqlDbType.Int, 1));
	foreach (var item in list)
	{
		Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
	}
}

4. Using the call Query<T>(string sql, Func<IDataReader, T> factory, params IParameter[] parameters)
using (var db = new Database("MyConnectionStringName"))
{
	 var list = db.Query<Member>(
		"select Id, FirstName, LastName from Member where Id=@Id", 
		r => {
			var i = new Member();
			i.Id = r.GetInt32(r.GetOrdinal("Id"));
			i.FirstName = r.GetString(r.GetOrdinal("FirstName"));
			i.LastName = r.GetString(r.GetOrdinal("LastName"));
			return i;
		},
		new Parameter("@Id", SqlDbType.Int, 2));
	foreach (var item in list)
	{
		Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
	}
}

