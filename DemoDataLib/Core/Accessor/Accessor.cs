using Dapper;
using DemoDataLib.Core.Attribute;
using DemoDataLib.Core.Interface;
using DemoDataLib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core.Accessor
{
    public abstract class Accessor<TParam> : DbAccess where TParam : ICustomParam
    {
        public Accessor(Connection connection, TParam param) : base(connection)
        {
            Param = param;
        }
        public TParam Param { get; set; }

        public string SqlStr { get; protected set; }
        public DynamicParameters Parameters { get; set; } = new DynamicParameters();

        protected abstract void GenerateSqlStr();

        protected virtual string GenerateWhere()
        {
            List<string> whereList = new List<string>();
            var type = Param.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<ColumnAttribute>();
                if (attr == null)
                    continue;

                var propertyType = property.PropertyType;
                string columnName;
                string paramName;

                if (string.IsNullOrEmpty(attr.TableName))
                {
                    columnName = $"{attr.Name}";
                    paramName = $"{attr.Name}";
                }
                else
                {
                    columnName = $"{attr.TableName}.{attr.Name}";
                    paramName = $"{attr.TableName}_{attr.Name}";
                }

                dynamic value = property.GetValue(Param);
                if (!ColumnIsRun(value))
                {
                    continue;
                }

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Range<>))
                {
                    object minValue = value.Min;
                    object maxValue = value.Max;

                    if (minValue != null)
                    {
                        string paramStr = $"{paramName}_Min";
                        whereList.Add($"{columnName}>=@{paramStr}");
                        Parameters.AddBasic(paramStr, minValue, attr.DbType);
                    }

                    if (maxValue != null)
                    {
                        string paramStr = $@"{paramName}_Max";
                        whereList.Add($"{columnName}<=@{paramStr}");
                        Parameters.AddBasic(paramStr, maxValue, attr.DbType);
                    }
                }
                else
                {
                    whereList.Add($"{columnName}=@{paramName}");
                    Parameters.AddBasic(paramName, (object)value, attr.DbType);

                }
            }

            return string.Join(" AND ", whereList);
        }

        protected static bool ColumnIsRun(object value)
        {
            if (Equals(value, default))
            {
                return false;
            }

            var valueType = value.GetType();

            if (valueType.IsEnum)
            {
                if (Equals(value, Enum.ToObject(valueType, 0)))
                {
                    return false;
                }
            }

            return true;
        }
    }

}
