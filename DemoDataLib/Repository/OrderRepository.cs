using DemoDataLib.Core;
using DemoDataLib.Data;
using DemoDataLib.Model;

namespace DemoDataLib.Repository
{
    public class OrderRepository : RepositoryAccess<Order, DemoContext>
    {
        public OrderRepository(Connection conn) : base(conn)
        {
        }

        public Order GetFullDatas(int id, DbAccess.LockTypes lockType)
        {
            GetByPrimaryKeyAfterEvent += OrderRepository_GetByPrimaryKeyAfterEvent;

            return GetByPrimaryKey(new Order { ID = id }, lockType);
        }

        private void OrderRepository_GetByPrimaryKeyAfterEvent(Order param, Order data, Core.Attribute.TableAttribute tableAttr, DataAccess<Order> table, DbAccess.LockTypes lockType)
        {
            data.Member = DbContext.Member.GetByPrimaryKey(new Member { ID = data.MemberId }, lockType);
            data.Commodities = DbContext.GetByFilter(new Filter.QueryCommoditiesByOrderId.Request(DbContext.Connection, new Filter.QueryCommoditiesByOrderId
            {
                OrderId = data.ID
            }, lockType)).Cast<Commodity>().ToList();
        }
    }
}
