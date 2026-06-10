/*
using System.Data;
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query.TypeHandlers;

public class EnumerableIntTypeHandler : TypeHandler<IEnumerable<int>>
{
    public override IEnumerable<int> Parse(object value)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return [];
        return JsonSerializer.Deserialize<List<int>>(value.ToString());
    }

    public override void SetValue(IDbDataParameter parameter, IEnumerable<int> value)
    {
        throw new NotImplementedException();
    }
}
*/
