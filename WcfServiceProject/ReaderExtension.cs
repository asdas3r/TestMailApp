using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WcfServiceProject
{
    public static class ReaderExtension
    {
        public static T GetSafeValue<T>(this SqlDataReader reader, int column)
        {
            if (reader.IsDBNull(column))
            {
                if (typeof(T) == typeof(string))
                    return (T) (object) string.Empty;
                return default(T);
            }

            var result = reader.GetValue(column);
            if (result is T)
            {
                return (T) result;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }
}