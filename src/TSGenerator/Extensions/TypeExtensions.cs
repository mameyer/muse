using System;
using System.Linq;

namespace TSGenerator.Extensions
{
    public static class TypeExtensions
    {
        public static string ToGenericTypeName(this Type t)
        {
            if (t == null)
                return null;

            if (!t.IsGenericType)
                return t.Name;

            string genericTypeName = t.GetGenericTypeDefinition().Name;
            if (string.IsNullOrEmpty(genericTypeName))
                return null;

            return genericTypeName[..genericTypeName.IndexOf('`')];
        }

        public static string ToGenericTypeString(this Type t)
        {
            if (t == null)
                return null;

            if (!t.IsGenericType)
                return t.Name;

            string genericTypeName = t.ToGenericTypeName();
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }
    }
}
