using System.Data;
using System.Data.SqlClient;

namespace SimpleDataAccess
{
    public class Parameter : IParameter
    {
        private SqlParameter _underlyingParameter;

        public Parameter(string name, SqlDbType type, object value, int size, byte precision, byte scale)
        {
            _underlyingParameter = new SqlParameter(name, value);
            switch(type)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    _underlyingParameter.Size = size;
                    break;
            }
            switch(type)
            {
                case SqlDbType.Decimal:
                    _underlyingParameter.Precision = precision;
                    _underlyingParameter.Scale = scale;
                    break;
            }
        }

        public void ApplyCommand(IDbCommand command)
        {
            command.Parameters.Add(_underlyingParameter);
        }
    }
}
