using DemoDataLib.Core.Accessor;
using DemoDataLib.Core.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DemoDataLib.Core
{
    public abstract class DbContext
    {
        [JsonIgnore]
        public Connection Connection { get; protected set; }
        public DbContext()
        {

        }

        public void Init(Connection conn)
        {
            Connection = conn;
            // 使用反射遍歷所有的DataAccess屬性
            var properties = GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DataAccess<>));

            foreach (var property in properties)
            {
                var genericType = property.PropertyType.GetGenericArguments().First();
                var dataAccessType = typeof(DataAccess<>).MakeGenericType(genericType);
                var dataAccess = Activator.CreateInstance(dataAccessType, conn);

                // 初始化DataAccess屬性
                property.SetValue(this, dataAccess);
            }
        }

        public List<TReturn> GetByFilter<TParam, TReturn>(Filter<TParam, TReturn> filter)
            where TParam : ICustomParam
            where TReturn : ICustomReturn
        {
            filter.SqlDatabase = Connection;
            return filter.Handle();
        }

        public TReturn HandleByActuator<TParam, TReturn>(Actuator<TParam, TReturn> actuator)
            where TParam : ICustomParam
            where TReturn : class
        {
            actuator.SqlDatabase = Connection;
            return actuator.Handle();
        }
    }

}
