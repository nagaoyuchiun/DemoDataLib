using Dapper;
using DemoDataLib.Core.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core
{
    public class DataAccess<TDbTable> : DbAccess where TDbTable : IDatabaseTable
    {

        public DataAccess(Connection connection) : base(connection)
        {
        }

        public TDbTable Table { get; set; }

        /// <summary>
        /// 資料表新增
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Insert(TDbTable table)
        {
            Table = table;


            //取得自身屬性
            var t = table.GetType();
            var props = t.GetProperties().ToList();

            List<string> createCoulmn = new List<string>();
            List<string> createProps = new List<string>();

            DynamicParameters dp = new DynamicParameters();

            foreach (var prop in props)
            {
                var propAttr = (Attribute.ColumnAccessAttribute)System.Attribute.GetCustomAttribute(prop, typeof(Attribute.ColumnAccessAttribute));
                if (propAttr == null)
                    continue;

                //從自身屬性去判斷，如果有新增忽略、棄用、系統屬性、關聯物件，則不列入新增
                if (IsCreateIgnore(propAttr))
                    continue;

                var propValue = prop.GetValue(table);
                if (propValue == null)
                    continue;

                createCoulmn.Add($"[{propAttr.Name}]");
                createProps.Add($"@{propAttr.Name}");

                dp.AddBasic(propAttr.Name, propValue, propAttr.DbType);
            }

            if (createCoulmn.Count > 0)
            {
                string insStr = $"INSERT INTO {TableAccess.GetFullName<TDbTable>} ({string.Join(",", createCoulmn)}) VALUES ({string.Join(",", createProps)});" +
                                $"SELECT SCOPE_IDENTITY()";

                ProcessLog["sqlStr"] = insStr;
                ProcessLog["param"] = JToken.FromObject(dp, JsonSetting);
                return SqlDatabase.Conn.ExecuteScalar<int>(insStr, dp, SqlDatabase.Trans);

            }
            else
            {
                throw new Exception("未定義欄位屬性");
            }


        }

        /// <summary>
        /// 資料表更新(只能更新單主鍵的表)
        /// </summary>
        /// <param name="old"></param>
        /// <param name="newer"></param>
        public void Update(TDbTable old, TDbTable newer)
        {
            Table = newer;

            var oldType = old.GetType();
            var newType = newer.GetType();

            var oldProps = oldType.GetProperties().ToList();
            var newProps = newType.GetProperties().ToList();

            List<string> updateList = new List<string>();
            string key = "";

            DynamicParameters dp = new DynamicParameters();

            foreach (var newProp in newProps)
            {
                var newValue = newProp.GetValue(newer);

                var oldProp = oldProps.Single(x => x.Name == newProp.Name);
                var oldValue = oldProp.GetValue(old);

                var attr = (Attribute.ColumnAccessAttribute)System.Attribute.GetCustomAttribute(newProp, typeof(Attribute.ColumnAccessAttribute));
                if (attr == null)
                    continue;

                if (IsPrimaryKey(attr))
                {
                    key = $" [{attr.Name}]=@{attr.Name} ";
                    dp.AddBasic(attr.Name, newValue, attr.DbType);
                }

                //從自身屬性去判斷，如果有更新忽略、棄用、系統屬性、關聯物件，則不列入更新
                if (IsUpdateIgnore(attr))
                    continue;

                //差異比對，有異動才更新
                if (Equals(oldValue, newValue))
                    continue;

                updateList.Add($"[{attr.Name}]=@{attr.Name}");

                dp.AddBasic(attr.Name, newValue, attr.DbType);
            }

            if (updateList.Count > 0)
            {
                string upStr = $@"UPDATE {TableAccess.GetFullName<TDbTable>} SET {string.Join(",", updateList)} WHERE {key}";

                ProcessLog["sqlStr"] = upStr;
                ProcessLog["param"] = JToken.FromObject(dp, JsonSetting);

                SqlDatabase.Conn.Execute(upStr, dp, SqlDatabase.Trans);
            }
        }

        public TDbTable GetByPrimaryKey(TDbTable table, LockTypes lockType)
        {
            Table = table;

            //取得自身屬性
            var t = table.GetType();

            var props = t.GetProperties().ToList();

            DynamicParameters dp = new DynamicParameters();
            List<string> selectStr = new List<string>();
            string whereStr = "";

            foreach (var prop in props)
            {
                var attr = (Attribute.ColumnAccessAttribute)System.Attribute.GetCustomAttribute(prop, typeof(Attribute.ColumnAccessAttribute));

                if (attr == null)
                    continue;

                var propValue = prop.GetValue(table);

                if (IsPrimaryKey(attr))
                {
                    whereStr = $"{attr.Name}=@{attr.Name}";
                    dp.AddBasic(attr.Name, propValue, attr.DbType);
                }

                //從自身屬性去判斷，如果有棄用、系統屬性、關聯物件，則不列入查詢目標欄位
                if (IsCommonIgnore(attr))
                    continue;
                selectStr.Add(attr.Name);
            }

            if (selectStr.Count > 0 && !string.IsNullOrEmpty(whereStr))
            {
                string qStr = $"SELECT {string.Join(",", selectStr)} FROM {TableAccess.GetFullName<TDbTable>} WITH({lockType}) WHERE {whereStr}";

                ProcessLog["sqlStr"] = qStr;
                ProcessLog["param"] = JToken.FromObject(dp, JsonSetting);
                return SqlDatabase.Conn.QueryFirstOrDefault<TDbTable>(qStr, dp, SqlDatabase.Trans);
            }

            return default;
        }

    }

}
