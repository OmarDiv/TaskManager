/*
using System.Data;
using System.Numerics;
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query.TypeHandlers;

public class ObjectIdTypeHandler<T> : ObjectIdTypeHandler<T, long> where T : ObjectId<long> { }

public class ObjectIdTypeHandler<T, TId> : TypeHandler<T> where T : ObjectId<TId> where TId : INumber<TId>, IParsable<TId>
{
    public override T? Parse(object value)
    {
        return value == null ? null : (T)Activator.CreateInstance(typeof(T), (TId)value);
    }

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value == null ? null : value.Id;
    }
}
*/
