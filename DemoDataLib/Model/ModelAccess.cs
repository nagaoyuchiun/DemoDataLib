using DemoDataLib.Core.Attribute;
using DemoDataLib.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DemoDataLib.Model
{
    public class ModelAccess : IDatabaseTable
    {
        [ColumnAccess(Name = "id", IsPrimaryKey = true, IsCreateIngore = true, IsUpdateIngore = true, DbType = DbType.Int32)]
        public int ID { get; set; }
    }
}
