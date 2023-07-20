using Dapper;
using DemoDataLib.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core
{
    public static class Extensions
    {
        public static void AddBasic(this DynamicParameters param, string name, object value, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Date:
                    param.Add(name, (DateTime)value, dbType);
                    break;
                case DbType.Object:
                    param.Add(name, value);
                    break;
                default:
                    param.Add(name, value, dbType);
                    break;
            }
        }

        public static string GetFullName(this IDatabaseTable param)
        {
            var t = param.GetType();
            var tableAttr = (Attribute.TableAttribute)System.Attribute.GetCustomAttribute(t, typeof(Attribute.TableAttribute));
            if (tableAttr == null)
                return "";

            return tableAttr.Name;
        }
    }
}
