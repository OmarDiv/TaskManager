using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Tasks.Commands.DeleteTask
{
    public record DeleteTask(long Id, long UserId) : IRequest<Result>;

    public class DeleteTaskHandler(
        IGenericRepository<ProjectTask> _taskRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<DeleteTask, Result>
    {
        public async Task<Result> Handle(DeleteTask request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetById(
                request.Id,
                q => q.Include(t => t.Project),
                cancellationToken
            );

            if (task == null)
            {
                return ResultMessage.TaskNotFound;
            }

            if (task.Project.CreatedById != request.UserId)
            {
                return ResultMessage.TaskUnauthorizedAccess;
            }

            await _taskRepository.DeleteAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveByPrefixAsync($"project-tasks-{task.ProjectId}-", cancellationToken);

            return Result.Success();
        }
    }
}
