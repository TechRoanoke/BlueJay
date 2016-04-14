using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    internal static class Utility
    {
        // Convert an object to a query parameter dictionary. 
        public static void ToQueryParams(IDictionary<string, string> queryParams, object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                var val = prop.GetValue(obj);
                if (val != null)
                {
                    queryParams.Add(prop.Name, val.ToString());
                }
            }
        }
    }
}
