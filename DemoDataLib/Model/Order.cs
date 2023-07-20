using DemoDataLib.Core.Attribute;
using System.Data;

namespace DemoDataLib.Model
{
    [Table(Name = "order")]
    public class Order : ModelAccess
    {
        [ColumnAccess(Name = "createDateTime", IsUpdateIngore = true, DbType = DbType.DateTime)]
        public DateTime CreateDateTime { get; set; }

        [ColumnAccess(Name = "memberId", IsUpdateIngore = true, DbType = DbType.Int32)]
        public int MemberId { get; set; }

        [ColumnAccess(ColumnType = ColumnAttribute.ColumnTypes.Reference)]
        public Member Member { get; set; }

        [ColumnAccess(ColumnType = ColumnAttribute.ColumnTypes.Reference)]
        public List<Commodity> Commodities { get; set; }
    }
}
