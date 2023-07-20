using DemoDataLib.Core;
using DemoDataLib.Model;

namespace DemoDataLib.Data
{
    public class DemoContext : DbContext
    {
        public DataAccess<Member> Member { get; set; }
        public DataAccess<Order> Order { get; set; }
        public DataAccess<Commodity> Commodity { get; set; }
    }
}
