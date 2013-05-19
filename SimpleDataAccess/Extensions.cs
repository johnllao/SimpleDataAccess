using System;

namespace DataAccess
{
    public static class Extensions
    {
        public static bool IsDynamic(this Type type)
        {
            string dynamicAttr = "__DynamicallyInvokableAttribute";
            var attibutes = type.GetCustomAttributes(false);
            for(var i = 0; i <= attibutes.Length - 1; i++)
            {
                if (attibutes[i].GetType().Name == dynamicAttr) return true;
            }
            return false;
        }
    }
}
