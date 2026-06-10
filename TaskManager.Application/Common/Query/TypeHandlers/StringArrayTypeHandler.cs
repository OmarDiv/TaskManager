/*
using Newtonsoft.Json;
using System.Data;
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query.TypeHandlers;

public class StringArrayTypeHandler : TypeHandler<string[]>
{
    public override string[]? Parse(object value)
    {
        return JsonConvert.DeserializeObject<string[]>(value.ToString());
    }

    public override void SetValue(IDbDataParameter parameter, string[]? value)
    {
        parameter.Value = JsonConvert.SerializeObject(value);
    }
}
*/
