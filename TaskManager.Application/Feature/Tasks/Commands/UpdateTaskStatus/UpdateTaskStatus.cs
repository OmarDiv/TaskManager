using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Tasks.Errors;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Commands.UpdateTaskStatus
{
    public record UpdateTaskStatus(
        long Id,
        Status Status,
        long UserId
    ) : IRequest<Result<TaskResponse>>;

    public class UpdateTaskStatusHandler(
        IGenericRepository<ProjectTask> _taskRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<UpdateTaskStatus, Result<TaskResponse>>
    {
        public async Task<Result<TaskResponse>> Handle(UpdateTaskStatus request, CancellationToken cancellationToken)
        {
            // Retrieve task and include Project to check ownership
            var task = await _taskRepository.GetById(
                request.Id,
                q => q.Include(t => t.Project),
                cancellationToken
            );

            if (task == null)
            {
                return Result.Failure<TaskResponse>(TaskErrors.NotFound);
            }

            if (task.Project.CreatedById != request.UserId)
            {
                return Result.Failure<TaskResponse>(TaskErrors.UnauthorizedAccess);
            }

            task.Status = request.Status;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveAsync($"project-tasks-{task.ProjectId}", cancellationToken);

            var response = task.Adapt<TaskResponse>();

            return Result.Success(response);
        }
    }
}
