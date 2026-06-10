/*
using Clinic.Application.Common.Services.ConnectionString;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Clinic.Application.Common.Services.Query;

public class QueryExecuter : IQueryExecuter
{
    private readonly string _connection;
    public QueryExecuter(IConnectionStringService connectionStringService)
    {
        _connection = connectionStringService.GetConnectionString(null);
    }

    public async Task<IEnumerable<T>> Query<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null)
    {
        var result = await ExecuteQuery(connection => connection.QueryAsync<T>(sql, param, null, commandTimeout, commandType), param, connectionString);
        var sensitiveProperties = GetSensitiveProperties<T>();
        if (sensitiveProperties.Any())
        {
            foreach (var item in result)
                DecryptSensitiveProperties(item, sensitiveProperties);
        }
        return result;
    }
    public async Task<PagedModel<T>> QueryPaginated<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null)
    {
        var pageNumber = param.GetIntPropertyValue("PageNumber") ?? 1;
        var pageSize = param.GetIntPropertyValue("PageSize") ?? 10;

        Func<IDbConnection, Task<IEnumerable<dynamic>>> queryExecutor = connection => connection.QueryAsync<dynamic>(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        var rawRecords = await ExecuteQuery(queryExecutor, param, connectionString);

        var records = rawRecords.Cast<IDictionary<string, object>>();

        var data = records.Select(x => x.Adapt<T>()).ToList();

        var totalRecords = records.FirstOrDefault()?["TotalRecords"] is object total ? Convert.ToInt32(total) : 0;

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalRecords / (double)pageSize) : 0;

        return new PagedModel<T>
        {
            ListData = data,
            PaginationData = new PaginationMetadata
            {
                TotalCount = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            }
        };
    }

    public async Task<T?> QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null)
    {
        var result = await ExecuteQuery(connection => connection.QueryFirstOrDefaultAsync<T>(sql, param, null, commandTimeout, commandType), param, connectionString);

        var sensitiveProperties = GetSensitiveProperties<T>();
        if (sensitiveProperties.Any())
            DecryptSensitiveProperties(result, sensitiveProperties);

        return result;
    }

    public Task<T?> ExecuteScalar<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null)
    {
        return ExecuteQuery(connection => connection.ExecuteScalarAsync<T>(sql, param, null, commandTimeout, commandType), param, connectionString);
    }

    public async Task<T?> QueryMultiple<T>(string sql, Func<SqlMapper.GridReader, Task<T?>> mapResultSets, object? param = null, int? commandTimeout = null, CommandType? commandType = null, string? connectionString = null)
    {
        var result = await ExecuteQuery(async connection =>
        {
            var reader = await connection.QueryMultipleAsync(sql, param, null, commandTimeout, commandType);
            var result = await mapResultSets(reader);
            return result;
        }, param, connectionString: connectionString);

        var sensitiveProperties = GetSensitiveProperties<T>();
        if (sensitiveProperties.Any())
            DecryptSensitiveProperties(result, sensitiveProperties);

        return result;
    }

    private async Task<T> ExecuteQuery<T>(Func<IDbConnection, Task<T>> executerFunc, object param, string? connectionString = null)
    {
        using (var connection = new SqlConnection(connectionString ?? _connection))
        {
            return await executerFunc(connection);
        }
    }
    private IEnumerable<PropertyInfo> GetSensitiveProperties<T>()
    {
        return typeof(T).GetProperties().Where(x => x.GetCustomAttribute<SensitiveAttribute>() != null).ToArray();
    }

    private static void DecryptSensitiveProperties(object item, IEnumerable<PropertyInfo> sensitiveProperties)
    {
        foreach (var property in sensitiveProperties)
            property.SetValue(item, ObjectIdObfuscator.Decrypt(property.GetValue(item)?.ToString()));
    }


}
*/
