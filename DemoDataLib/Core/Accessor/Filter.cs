using Dapper;
using DemoDataLib.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoDataLib.Core.Accessor
{
    public abstract class Filter<TParam, TReturn> : Accessor<TParam>
        where TParam : ICustomParam
        where TReturn : ICustomReturn
    {
        protected LockTypes LockType { get; set; }

        public Filter(Connection connection, TParam param, LockTypes lockType) : base(connection, param)
        {
            LockType = lockType;

            GenerateSqlStr();
            GenerateWhere();
        }

        public class GenerateWhereObjResponse
        {
            public DynamicParameters WhereParameters { get; set; }
            public string WhereString { get; set; }
        }

        protected virtual string GenerateSelectStr()
        {
            var t = typeof(TReturn);
            var props = t.GetProperties().ToList();

            List<string> selectTargets = new List<string>();

            foreach (var prop in props)
            {
                var propAttr = (Attribute.ColumnAttribute)System.Attribute.GetCustomAttribute(prop, typeof(Attribute.ColumnAttribute));
                if (propAttr == null)
                    continue;

                //從自身屬性去判斷，如果有新增忽略、棄用、系統屬性、關聯物件，則不列入新增
                if (IsCommonIgnore(propAttr))
                    continue;

                if (string.IsNullOrEmpty(propAttr.TableName))
                    selectTargets.Add(propAttr.Name);
                else
                    selectTargets.Add($"{propAttr.TableName}.{propAttr.Name}");
            }

            if (selectTargets.Count > 0)
            {
                return $"SELECT {string.Join(",", selectTargets)}";
            }
            else
            {
                return "SELECT *";
            }

        }

        protected virtual string GenerateFormStr()
        {
            throw new NotImplementedException();
        }
        protected override void GenerateSqlStr()
        {
            string whereString = GenerateWhere();
            SqlStr = "";
            SqlStr += $" {GenerateSelectStr().Trim()} ";
            SqlStr += $" {GenerateFormStr().Trim()} ";
            if (!string.IsNullOrEmpty(whereString))
            {
                SqlStr += $" WHERE {whereString.Trim()} ";
            }
        }

        public List<TReturn> Handle()
        {
            return SqlDatabase.Conn.Query<TReturn>(SqlStr, Parameters, SqlDatabase.Trans).ToList();
        }

    }
}
