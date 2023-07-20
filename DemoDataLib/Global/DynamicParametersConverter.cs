using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Global
{
    public class DynamicParametersConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DynamicParameters);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DynamicParameters parameters = (DynamicParameters)value;
            var dict = new Dictionary<string, object>();
            foreach (string name in parameters.ParameterNames)
            {
                dict.Add(name, parameters.Get<object>(name));
            }
            serializer.Serialize(writer, dict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
