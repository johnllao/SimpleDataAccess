using System;
using System.Data;

namespace DataAccess
{
    public interface IParameter
    {
        void ApplyCommand(IDbCommand command);
    }
}
