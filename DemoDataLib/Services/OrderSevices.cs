using DemoDataLib.Core;
using DemoDataLib.Repository;
using DemoDataLib.Services.Excute;

namespace DemoDataLib.Services
{
    public class OrderSevices : ServiceAccess
    {
        OrderRepository orderRepos;
        public OrderSevices(Connection conn) : base(conn)
        {
            orderRepos = new OrderRepository(conn);
        }

        public ExcuteFunc<OrderSevices, Model.Order> QueryOrderWithAllInfo(int orderId)
        {
            return new ExcuteFunc<OrderSevices, Model.Order>((x) =>
            {
                return orderRepos.GetFullDatas(orderId, DbAccess.LockTypes.NOLOCK);
            });
        }
    }
}
