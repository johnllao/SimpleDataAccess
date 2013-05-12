using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public interface IDatabase
    {
        void Execute(string sql, params IParameter[] parameters);
        IEnumerable<T> Query<T>(string sql, params IParameter[] parameters);
        IEnumerable<T> Query<T>(string sql, Func<IDataReader, T> factory, params IParameter[] parameters);
    }
}
