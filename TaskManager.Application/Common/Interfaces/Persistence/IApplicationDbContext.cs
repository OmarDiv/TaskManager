using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TaskManager.Application.Common.Interfaces.Persistence
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
