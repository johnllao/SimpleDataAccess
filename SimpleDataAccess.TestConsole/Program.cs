using System;
using System.Data;
using System.Diagnostics;

namespace DataAccess.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            using (var db = new Database("Island"))
            {
                Console.WriteLine("Using the call Execute(string sql, params IParameter[] parameters)");
                db.Execute("insert into Member (FirstName, LastName) values (@firstName, @lastName)",
                    new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
                    new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));

                Console.WriteLine();
                Console.WriteLine("Using the call Query<T>(string sql, params IParameter[] parameters)");
                var list1 = db.Query<Member>("select Id, FirstName, LastName from Member where Id=@Id", new Parameter("@Id", SqlDbType.Int, 1));
                foreach (var item in list1)
                {
                    Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
                }

                Console.WriteLine();
                Console.WriteLine("Using the call Query<T>(string sql, Func<IDataReader, T> factory, params IParameter[] parameters)");
                var list2 = db.Query<Member>(
                    "select Id, FirstName, LastName from Member where Id=@Id", 
                    r => {
                        var i = new Member();
                        i.Id = r.GetInt32(r.GetOrdinal("Id"));
                        i.FirstName = r.GetString(r.GetOrdinal("FirstName")) + " :)";
                        i.LastName = r.GetString(r.GetOrdinal("LastName")) + " :)";
                        return i;
                    },
                    new Parameter("@Id", SqlDbType.Int, 2));
                foreach (var item in list2)
                {
                    Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
                }

                Console.WriteLine();
                Console.WriteLine("Using the call Query<T>(string sql)");
                var list3 = db.Query<Member>("select Id, FirstName, LastName from Member");
                foreach (var item in list3)
                {
                    Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
                }

                db.Execute("delete from Member where FirstName = @firstName and LastName = @lastName",
                    new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
                    new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));

            }

            Console.WriteLine();
            Console.WriteLine("Using the transactions");
            using (var db = new Database("Island"))
            {
                db.BeginTransaction();
                db.Execute("insert into Member (FirstName, LastName) values (@firstName, @lastName)",
                   new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
                   new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50));
                db.RollbackTransaction();

                var list = db.Query<Member>(
                    "select Id, FirstName, LastName from Member where FirstName=@firstName and LastName=@LastName", 
                    r => {
                        var i = new Member();
                        i.Id = r.GetInt32(r.GetOrdinal("Id"));
                        i.FirstName = r.GetString(r.GetOrdinal("FirstName"));
                        i.LastName = r.GetString(r.GetOrdinal("LastName"));
                        return i;
                    },
                    new Parameter("@firstName", SqlDbType.VarChar, "John", 50),
                    new Parameter("@lastName", SqlDbType.VarChar, "Smith", 50)
                );
                foreach (var item in list)
                {
                    Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            Console.ReadLine();

        }
    }

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
}
