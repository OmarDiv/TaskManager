using Microsoft.EntityFrameworkCore.Storage;
using TaskManager.Application.Common.Interfaces.Repositories;

namespace TaskManager.Application.Common.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}
