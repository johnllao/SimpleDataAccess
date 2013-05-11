using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SimpleDataAccess
{
    public class Database : IDisposable
    {
        private string _connectionString;
        private string _connectionStringProviderName;
        private IDbConnection _connection;
        private static Dictionary<Type, Delegate> _factoryCache = new Dictionary<Type,Delegate>();

        public Database(string connectionStringName) : this(connectionStringName, 30) { }

        public Database(string connectionStringName, int timeout)
        {
            if (connectionStringName == null)
                throw new ArgumentNullException("connectionStringName");

            if (timeout <= 0)
                throw new ArgumentException(string.Format("The timeout value cannot be zero or negative value"), "timeout");

            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString == null)
                throw new NullReferenceException(string.Format("Connection string {0} not found in the configuration", connectionStringName));

            _connectionString = connectionString.ConnectionString;
            _connectionStringProviderName = connectionString.ProviderName;
        }

        public IEnumerable<T> Query<T>(string sql, params Parameter[] parameters)
        {
            var factory = GetFactory<T>() as Func<IDataReader, T>;
            return Query<T>(sql, factory, parameters);
        }

        public IEnumerable<T> Query<T>(string sql, Func<IDataReader, T> factory, params Parameter[] parameters)
        {
            OpenConnection();
            using (var cmd = CreateCommand(sql, parameters))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var o = factory(reader);
                        yield return o;
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        private void OpenConnection()
        {
            if (_connection == null)
            {
                if (string.IsNullOrEmpty(_connectionStringProviderName))
                    throw new NullReferenceException(string.Format("Open connection failed. Connection string provider name {0} not found in the configuration", _connectionStringProviderName));

                var factory = DbProviderFactories.GetFactory(_connectionStringProviderName);
                if (factory == null)
                    throw new NullReferenceException(string.Format("Open connection failed. Cannot create instance of the connection factory. Provider name {0} in the connectin string could be incorrect", _connectionStringProviderName));

                _connection = factory.CreateConnection();
                if (_connection == null)
                    throw new NullReferenceException(string.Format("Open connection failed. Cannot create instance of the connection using the factory. Provider name {0} in the connectin string could be incorrect", _connectionStringProviderName));

                _connection.ConnectionString = _connectionString;
                _connection.Open();
            }
        }

        private IDbCommand CreateCommand(string sql, params Parameter[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(sql);

            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    parameter.ApplyCommand(cmd);
                }
            }
            return cmd;
        }

        private static Delegate GetFactory<T>()
        {
            Delegate factory;
            Type type = typeof(T);
            if (_factoryCache.ContainsKey(type))
            {
                factory = _factoryCache[type];
                return factory;
            }

            var factoryMethod = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            if (factoryMethod == null)
                throw new NullReferenceException(string.Format("Unable to retrieve the factory method of class {0}. Please consider having a static method 'Create' in the class", type.Name));

            factory = Delegate.CreateDelegate(typeof(Func<IDataReader, T>), factoryMethod);
            _factoryCache.Add(type, factory);
            return factory;
        }
    }
}
