using System;

namespace Mvc.Stream.Internal
{
    internal static class Extensions
    {
        public static long ToLong(this string str)
        {
            return str.ToLong(default(long));
        }

        public static long ToLong(this string str, long defaultResult)
        {
            long result;
            if(!long.TryParse(str, out result))
            {
                result = defaultResult;
            }
            return result;
        }

        public static bool EqualsCaseInsensetive(this string str, string other)
        {
            return str.Equals(other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
