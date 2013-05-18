Simple Data Access
================

Simple data access library for .NET. Code is inspired by peta POCO and Massive. Goal is to be able to run SQL queries and be able to return concrete objects

Create a class with a factory method
----------------

```csharp
public class Member
{
	public int Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }

	public static Member Create(IDataReader reader)
	{
		var i = new Member();
		i.Id = reader.GetInt32(reader.GetOrdinal("Id"));
		i.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
		i.LastName = reader.GetString(reader.GetOrdinal("LastName"));
		return i;
	}
}
```

Using the call Execute(string sql, params IParameter[] parameters)
----------------

```csharp
using (var db = new Database("MyConnectionStringName"))
{
	db.Execute("insert into Member (FirstName, LastName) values (@firstName, @lastName)",
		new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
		new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));
}
```

Using the call Query<T>(string sql)
----------------

```csharp
using (var db = new Database("MyConnectionStringName"))
{
	var list = db.Query<Member>("select Id, FirstName, LastName from Member");
	foreach (var item in list)
	{
		Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
	}
}
```

Using the call Query<T>(string sql, params IParameter[] parameters)
----------------

```csharp
using (var db = new Database("MyConnectionStringName"))
{
	var list = db.Query<Member>(
		"select Id, FirstName, LastName from Member where Id=@Id", 
		new Parameter("@Id", SqlDbType.Int, 1));
	foreach (var item in list)
	{
		Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
	}
}
```

Using the call Query<T>(string sql, Func&lt;IDataReader, T&gt; factory, params IParameter[] parameters)
----------------

```csharp
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
```

Using Transactions
----------------

```csharp
using (var db = new Database("MyConnectionStringName"))
{
	db.BeginTransaction();
	db.Execute("insert into Member (FirstName, LastName) values (@firstName, @lastName)",
	   new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
	   new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));
	db.RollbackTransaction();
}
```
