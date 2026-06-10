/*
using System.Data;
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query.TypeHandlers;

public class AnyTypeHandler<T> : TypeHandler<T>
{
    public override T Parse(object value)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return default;
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}
*/
