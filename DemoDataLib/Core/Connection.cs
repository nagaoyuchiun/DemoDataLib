using Newtonsoft.Json;
using System.Data.SqlClient;

namespace DemoDataLib.Core
{
    public class Connection
    {
        [JsonIgnore]
        public SqlConnection Conn { get; set; }

        [JsonIgnore]
        public SqlTransaction Trans { get; set; }

        public Connection()
        {

        }

        public Connection(SqlConnection connection)
        {
            Conn = connection;
        }

        public Connection(SqlTransaction transaction)
        {
            Conn = transaction.Connection;
            Trans = transaction;
        }
    }

}
