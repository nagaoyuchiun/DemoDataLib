using DemoDataLib.Core.Attribute;
using DemoDataLib.Core.Interface;
using DemoDataLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core
{
    public class TableAccess
    {
        public static string GetFullName<T>() where T : IDatabaseTable
        {
            var t = typeof(T);
            var tableAttr = (TableAttribute)System.Attribute.GetCustomAttribute(t, typeof(TableAttribute));
            if (tableAttr == null)
                return "";

            return tableAttr.Name;
        }
    }
}
