using System;
using System.Data;

namespace SimpleDataAccess
{
    public interface IParameter
    {
        void ApplyCommand(IDbCommand command);
    }
}
