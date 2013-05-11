using System;
using System.Data;
using System.Diagnostics;

namespace SimpleDataAccess.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            using (var db = new Database("Island"))
            {
                var list1 = db.Query<Member>("select Id, FirstName, LastName from Member where Id=@Id", new Parameter("@Id", SqlDbType.Int, 1));
                foreach (var item in list1)
                {
                    Console.WriteLine("[{0}] {1} {2}", item.Id, item.FirstName, item.LastName);
                }

                var list2 = db.Query<Member>("select Id, FirstName, LastName from Member where Id=@Id", new Parameter("@Id", SqlDbType.Int, 2));
                foreach (var item in list2)
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
