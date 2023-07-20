using DemoDataLib.Core;
using DemoDataLib.Core.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Model
{
    [Table(Name = "commodity")]
    public class Commodity : ModelAccess
    {

        [ColumnAccess(Name = "createDateTime", IsUpdateIngore = true, DbType = DbType.DateTime)]
        public DateTime CreateDateTime { get; set; }

        [ColumnAccess(Name = "name", DbType = DbType.String)]
        public string Name { get; set; }

        [ColumnAccess(Name = "price", DbType = DbType.Decimal)]
        public decimal Price { get; set; }

        [ColumnAccess(ColumnType = ColumnAttribute.ColumnTypes.Reference)]
        public List<Order> Orders { get; set; }
    }
}
