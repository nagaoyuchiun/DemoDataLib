using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using DemoDataLib.Global;

namespace DemoDataLib.Core
{
    public class DbAccess
    {
        protected readonly JsonSerializer JsonSetting = new JsonSerializer();
        public JToken ProcessLog { get; set; } = new JObject();

        public enum LockTypes
        {
            NOLOCK,
            READPAST,
            UPDLOCK,
            HOLDLOCK,
            XLOCK
        }

        [JsonIgnore]
        public Connection SqlDatabase { get; set; }

        public DbAccess(Connection connection)
        {
            SqlDatabase = connection;
            JsonSetting.Converters.Add(new DynamicParametersConverter());
            JsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }


        protected bool IsUpdateIgnore(Attribute.ColumnAccessAttribute attr)
        {
            if (attr == null)
                return true;
            if (attr.IsUpdateIngore)
                return true;
            if (IsCommonIgnore(attr))
                return true;

            return false;
        }

        protected bool IsCreateIgnore(Attribute.ColumnAccessAttribute attr)
        {
            if (attr == null)
                return true;
            if (attr.IsCreateIngore)
                return true;
            if (IsCommonIgnore(attr))
                return true;

            return false;
        }

        protected bool IsCommonIgnore(Attribute.ColumnAttribute attr)
        {
            if (attr.IsDeprecation)
                return true;
            if (attr.ColumnType == Attribute.ColumnAttribute.ColumnTypes.Reference)
                return true;
            if (attr.ColumnType == Attribute.ColumnAttribute.ColumnTypes.SystemProperty)
                return true;

            return false;

        }

        protected bool IsPrimaryKey(Attribute.ColumnAccessAttribute attr)
        {
            if (attr == null)
                return false;
            if (attr.IsPrimaryKey)
                return true;

            return false;

        }


    }

}
