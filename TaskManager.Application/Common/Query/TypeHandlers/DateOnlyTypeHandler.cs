/*
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query.TypeHandlers;

public class DateOnlyTypeHandler : TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime((DateTime)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = System.Data.DbType.Date;
        parameter.Value = new DateTime(value.Year, value.Month, value.Day);
    }
}
*/
