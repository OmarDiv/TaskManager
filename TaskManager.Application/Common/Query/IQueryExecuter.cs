/*
using System.Data;
using static Dapper.SqlMapper;

namespace Clinic.Application.Common.Services.Query;

public interface IQueryExecuter
{
    Task<IEnumerable<T>> Query<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null);
    Task<T?> QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null);
    Task<T?> ExecuteScalar<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null);
    Task<T?> QueryMultiple<T>(string sql, Func<GridReader, Task<T?>> mapResultSets, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null);
    Task<PagedModel<T>> QueryPaginated<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null);
}
*/
