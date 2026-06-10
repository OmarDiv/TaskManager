/*
using Clinic.Application.Interfaces.Persistence;
using Clinic.Domain.EntityModules.Common;
using Clinic.Domain.EntityModules.Common.Localizations;
using System.Linq.Expressions;

namespace Clinic.Application.Common.Services.Query;

public interface IEntityQueryService
{
    Task<bool> HasForeignKeyReferences(string tableName, long primaryKeyValue);
    Task<bool> IsLocalizationValueExists<TEntity>(IBaseRepository<TEntity> repository, Expression<Func<TEntity, LocalizationSet>> localizationSelector, List<LocalizationDto> localizations, long? excludeId = null, Expression<Func<TEntity, bool>>? additionalConditions = null, CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    Task<bool> IsValuesExists(string tableName, object columnValues, object? otherColumnValues = null, long? excludedId = null, SearchTypeEnum columnValuesOperator = SearchTypeEnum.Or, SearchTypeEnum otherColumnValuesOperator = SearchTypeEnum.And);
    Task<bool> IsActiveEntity(string tableName, long id);
    Task<Dictionary<long, bool>> AreEntitiesActive(string tableName, IEnumerable<long> ids);
    Task<bool> AreAllEntitiesActive(Dictionary<string, long> entityTableIds);
    Task<Dictionary<string, bool>> GetEntitiesActiveStatus(Dictionary<string, long> entityTableIds);
}
*/
