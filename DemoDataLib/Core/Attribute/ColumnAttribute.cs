using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core.Attribute
{
    public class ColumnAttribute : System.Attribute
    {

        public enum ColumnTypes
        {
            Common = 1, Reference = 2, SystemProperty = 3
        }

        public string Name { get; set; }
        public string Description { get; set; } = "";
        public bool IsDeprecation { get; set; } = false;
        public ColumnTypes ColumnType { get; set; } = ColumnTypes.Common;

        //預設為Object，但Object不會帶入Dapper的DynamicParameter的定義中，讓其動態對映
        public DbType DbType { get; set; } = DbType.Object;
        public string DbTypeString { get; set; } = "";

        public string TableName { get; set; } = "";
    }

}
