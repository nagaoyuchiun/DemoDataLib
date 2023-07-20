using DemoDataLib.Core;
using DemoDataLib.Core.Attribute;
using DemoDataLib.Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Model
{
    [Table(Name = "member")]
    public class Member : ModelAccess
    {
        public enum Sexes
        {
            Male,
            Female
        }

        [ColumnAccess(Name = "createDateTime", IsUpdateIngore = true, DbType = DbType.DateTime)]
        public DateTime CreateDateTime { get; set; }

        [ColumnAccess(Name = "userName", IsUpdateIngore = true, DbType = DbType.String)]
        public string UserName { get; set; }

        [ColumnAccess(Name = "password", IsUpdateIngore = true, DbType = DbType.String)]
        public string Password { get; set; }

        [ColumnAccess(Name = "sex", DbType = DbType.Int32)]
        public Sexes Sex { get; set; }

        [ColumnAccess(Name = "birthday", DbType = DbType.Int32)]
        public DateTime Birthday { get; set; }

        [ColumnAccess(ColumnType = ColumnAttribute.ColumnTypes.SystemProperty)]
        public long BirthdayUnixTime
        {
            get
            {
                return Birthday.GetUnixTime();
            }
        }
    }
}
