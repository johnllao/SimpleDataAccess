using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SimpleDataAccess.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new Database("Island"))
            {
                var q = db.Query<Member>("select * from Member");
            }
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
