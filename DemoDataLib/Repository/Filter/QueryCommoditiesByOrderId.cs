using DemoDataLib.Core;
using DemoDataLib.Core.Attribute;
using DemoDataLib.Core.Interface;
using DemoDataLib.Model;
using System.Data;

namespace DemoDataLib.Repository.Filter
{
    public class QueryCommoditiesByOrderId : ICustomParam
    {
        [Column(Name = "id", DbType = DbType.Int32, TableName = "t3")]
        public int OrderId { get; set; }

        public class Return : Commodity, ICustomReturn
        {
        }

        public class Request : Core.Accessor.Filter<QueryCommoditiesByOrderId, Return>
        {
            public Request(Connection connection, QueryCommoditiesByOrderId param, LockTypes lockType) : base(connection, param, lockType)
            {
            }

            protected override string GenerateSelectStr()
            {
                return "SELECT DISTINCT t1.*";
            }

            protected override string GenerateFormStr()
            {
                return $" FROM {TableAccess.GetFullName<Commodity>()} AS t1 WITH({LockType})" +
                    $" JOIN {TableAccess.GetFullName<OrderCommodity>()} AS t2 WITH({LockType}) ON t2.id=t1.commodityId" +
                    $" JOIN {TableAccess.GetFullName<Order>()} AS t3 WITH({LockType}) ON t3.id=t2.orderId";
            }
        }

    }
}
