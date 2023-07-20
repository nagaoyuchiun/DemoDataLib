using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Core.Attribute
{
    public class ColumnAccessAttribute : ColumnAttribute
    {
        public bool IsPrimaryKey { get; set; } = false;
        public bool IsCreateIngore { get; set; } = false;
        public bool IsUpdateIngore { get; set; } = false;

    }
}
