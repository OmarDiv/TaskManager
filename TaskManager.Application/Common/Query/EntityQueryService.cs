/*
using Clinic.Application.Interfaces.Persistence;
using Clinic.Domain.EntityModules.Common;
using Clinic.Domain.EntityModules.Common.Localizations;
using System.Linq.Expressions;

namespace Clinic.Application.Common.Services.Query;

public class EntityQueryService(IQueryExecuter queryExecuter) : IEntityQueryService
{

    public async Task<bool> HasForeignKeyReferences(string tableName, long primaryKeyValue)
    {
        var query = StringUtils.FormatPlaceholders(SQLQueries.CheckForeignKeyReferences,
           new Dictionary<string, object>
           {
                { "TableName" , tableName},
                { "Id" , primaryKeyValue }
           });

        var isExist = await queryExecuter.ExecuteScalar<bool>(query);

        return isExist;
    }
    public async Task<bool> IsValuesExists(string tableName, object columnValues, object? otherColumnValues = null, long? excludedId = null, SearchTypeEnum columnValuesOperator = SearchTypeEnum.Or, SearchTypeEnum otherColumnValuesOperator = SearchTypeEnum.And)
    {
        if (columnValues == null)
            throw new ArgumentNullException(nameof(columnValues));

        var parameters = new Dictionary<string, object>();
        var whereParts = new List<string>();

        AddConditions(columnValues, "", columnValuesOperator, parameters, whereParts);

        if (otherColumnValues != null)
            AddConditions(otherColumnValues, "other_", otherColumnValuesOperator, parameters, whereParts);

        // الاستبعاد بالـ Id
        if (excludedId.HasValue)
        {
            whereParts.Add("[Id] != @excludedId");
            parameters["excludedId"] = excludedId.Value;
        }

        var whereClause = string.Join(" AND ", whereParts); // الربط بين أجزاء الشرط الكلية
        var sql = $"SELECT TOP 1 1 FROM [{tableName}] WHERE {whereClause}";

        return await queryExecuter.ExecuteScalar<bool>(sql, parameters);
    }
    public async Task<bool> IsLocalizationValueExists<TEntity>(IBaseRepository<TEntity> repository, Expression<Func<TEntity, LocalizationSet>> localizationSelector, List<LocalizationDto> localizations, long? excludeId = null, Expression<Func<TEntity, bool>>? additionalConditions = null, CancellationToken cancellationToken = default) where TEntity : class, IEntity
    {
        if (localizations?.Any() != true) return false;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var localizationSet = Expression.Invoke(localizationSelector, parameter);
        var localizationCollection = Expression.Property(localizationSet, nameof(LocalizationSet.Localization));

        // Build localization match conditions
        Expression? matchConditions = null;
        foreach (var localization in localizations)
        {
            var locParam = Expression.Parameter(typeof(Localization), "l");

            var keyMatch = Expression.Equal(
                Expression.Property(locParam, nameof(Localization.Key)),
                Expression.Constant(localization.Key));

            var valueMatch = Expression.Equal(
                CreateNormalizedStringExpression(Expression.Property(locParam, nameof(Localization.Value))),
                Expression.Constant(localization.Value.Trim().ToLower()));

            var condition = Expression.AndAlso(keyMatch, valueMatch);
            var anyLambda = Expression.Lambda<Func<Localization, bool>>(condition, locParam);
            var anyCall = Expression.Call(GetAnyMethod(), localizationCollection, anyLambda);

            matchConditions = matchConditions == null ? anyCall : Expression.OrElse(matchConditions, anyCall);
        }

        // Build exclude condition
        var excludeCondition = excludeId == null
            ? (Expression)Expression.Constant(true)
            : Expression.NotEqual(Expression.Property(parameter, nameof(IEntity.Id)), Expression.Constant(excludeId.Value));

        // Combine all conditions
        var finalCondition = Expression.AndAlso(matchConditions!, excludeCondition);

        if (additionalConditions != null)
        {
            var additionalInvoked = Expression.Invoke(additionalConditions, parameter);
            finalCondition = Expression.AndAlso(finalCondition, additionalInvoked);
        }

        var lambda = Expression.Lambda<Func<TEntity, bool>>(finalCondition, parameter);
        return await repository.AsQueryable().AnyAsync(lambda, cancellationToken);
    }

    public async Task<bool> IsActiveEntity(string tableName, long id)
    {
        //TODO make it work with ISDeleted
        return await queryExecuter.ExecuteScalar<bool>($"SELECT IsActive FROM {tableName} WHERE Id = @id", new
        {
            id
        });
    }

    public async Task<Dictionary<long, bool>> AreEntitiesActive(string tableName, IEnumerable<long> ids)
    {
        if (ids == null || !ids.Any())
            return new Dictionary<long, bool>();

        var idsList = ids.ToList();
        var idsParameter = string.Join(',', idsList);

        //TODO make it work with ISDeleted
        var sql = $"SELECT Id, IsActive FROM {tableName} WHERE Id IN (SELECT value FROM STRING_SPLIT(@ids, ','))";

        var results = await queryExecuter.Query<(long Id, bool IsActive)>(sql, new { ids = idsParameter });
        var resultDict = results.ToDictionary(r => r.Id, r => r.IsActive);

        // Add missing IDs as false (not found = not active)
        foreach (var id in idsList)
        {
            if (!resultDict.ContainsKey(id))
                resultDict[id] = false;
        }

        return resultDict;
    }
    public async Task<bool> AreAllEntitiesActive(Dictionary<string, long> entityTableIds)
    {
        if (entityTableIds == null || !entityTableIds.Any())
            return true;

        var unionQueries = new List<string>();
        var parameters = new Dictionary<string, object>();
        int paramIndex = 0;

        foreach (var entity in entityTableIds)
        {
            var paramName = $"id{paramIndex}";
            unionQueries.Add($"SELECT CASE WHEN IsActive = 1 THEN 1 ELSE 0 END as IsActive FROM [{entity.Key}] WHERE Id = @{paramName}");
            parameters[paramName] = entity.Value;
            paramIndex++;
        }

        var sql = string.Join(" UNION ALL ", unionQueries);
        var finalQuery = $"SELECT CASE WHEN COUNT(*) = SUM(IsActive) AND COUNT(*) = {entityTableIds.Count} THEN 1 ELSE 0 END as AllActive FROM ({sql}) as Results";

        return await queryExecuter.ExecuteScalar<bool>(finalQuery, parameters);
    }
    public async Task<Dictionary<string, bool>> GetEntitiesActiveStatus(Dictionary<string, long> entityTableIds)
    {
        if (entityTableIds == null || !entityTableIds.Any())
            return new Dictionary<string, bool>();

        var unionQueries = new List<string>();
        var parameters = new Dictionary<string, object>();
        int paramIndex = 0;

        foreach (var entity in entityTableIds)
        {
            var paramName = $"id{paramIndex}";
            unionQueries.Add($"SELECT '{entity.Key}' as TableName, CASE WHEN IsActive = 1 THEN 1 ELSE 0 END as IsActive FROM [{entity.Key}] WHERE Id = @{paramName}");
            parameters[paramName] = entity.Value;
            paramIndex++;
        }

        var sql = string.Join(" UNION ALL ", unionQueries);
        var results = await queryExecuter.Query<(string TableName, bool IsActive)>(sql, parameters);

        var resultDict = results.ToDictionary(r => r.TableName, r => r.IsActive);

        // Add missing entities as false (not found = not active)
        foreach (var entity in entityTableIds)
        {
            if (!resultDict.ContainsKey(entity.Key))
                resultDict[entity.Key] = false;
        }

        return resultDict;
    }
    private static Expression CreateNormalizedStringExpression(Expression stringExpression)
    {
        var trimMethod = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)!;
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        return Expression.Call(Expression.Call(stringExpression, trimMethod), toLowerMethod);
    }

    private static MethodInfo GetAnyMethod()
    {
        return typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Localization));
    }

    private void AddConditions(object values, string prefix, SearchTypeEnum op, Dictionary<string, object> parameters, List<string> whereParts)
    {
        var props = values.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var conditions = new List<string>();

        foreach (var prop in props)
        {
            var paramName = $"{prefix}{prop.Name}";
            conditions.Add($"[{prop.Name}] = @{paramName}");
            parameters[paramName] = prop.GetValue(values);
        }

        if (conditions.Any())
            whereParts.Add("(" + string.Join($" {op} ", conditions) + ")");
    }
}
*/
