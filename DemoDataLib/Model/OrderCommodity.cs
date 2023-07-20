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
    [Table(Name = "orderCommodity")]
    public class OrderCommodity : ModelAccess
    {
        [ColumnAccess(Name = "orderId", IsUpdateIngore = true, DbType = DbType.Int32)]
        public int OrderId { get; set; }

        [ColumnAccess(Name = "commodityId", IsUpdateIngore = true, DbType = DbType.Int32)]
        public int CommodityId { get; set; }
    }
}
