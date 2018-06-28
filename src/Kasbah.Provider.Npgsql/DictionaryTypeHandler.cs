using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Dapper.SqlMapper;

namespace Kasbah.Provider.Npgsql
{
    public class DictionaryTypeHandler : TypeHandler<JObject>
    {
        public override JObject Parse(object value)
        {
            return JsonConvert.DeserializeObject<JObject>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, JObject value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }
    }
}
