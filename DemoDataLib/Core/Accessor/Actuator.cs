using Dapper;
using DemoDataLib.Core.Accessor;
using System.Collections.Generic;
using DemoDataLib.Core.Interface;

namespace DemoDataLib.Core.Accessor
{
    public abstract class Actuator<TParam, TReturn> : Accessor<TParam>
        where TParam : ICustomParam
        where TReturn : class
    {
        protected Actuator(Connection connection, TParam param) : base(connection, param)
        {
            GenerateSqlStr();
            GenerateWhere();
        }

        public TReturn Handle()
        {
            return SqlDatabase.Conn.ExecuteScalar<TReturn>(SqlStr, Parameters, SqlDatabase.Trans);
        }

        public void HandleNoReturn()
        {
            SqlDatabase.Conn.Execute(SqlStr, Parameters, SqlDatabase.Trans);
        }
    }
}
