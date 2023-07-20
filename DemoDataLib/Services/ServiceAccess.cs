using DemoDataLib.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Services
{
    public class ServiceAccess
    {
        [JsonIgnore]
        public Connection Connection { get; set; }
        public ServiceAccess(Connection conn)
        {
            Connection = conn;
        }
    }
}
