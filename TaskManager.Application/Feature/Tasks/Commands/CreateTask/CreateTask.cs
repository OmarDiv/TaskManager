using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Commands.CreateTask
{
    public record CreateTask(
        string Title,
        string Description,
        Status Status,
        DateTime? DueDate,
        Priority Priority,
        long ProjectId,
        long CurrentUserId
    ) : IRequest<Result<TaskResponse>>;

    public class CreateTaskHandler(
        IGenericRepository<Project> _projectRepository,
        IGenericRepository<ProjectTask> _taskRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<CreateTask, Result<TaskResponse>>
    {
        public async Task<Result<TaskResponse>> Handle(CreateTask request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the current user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            if (project == null)
            {
                return Result.Failure<TaskResponse>(ProjectErrors.NotFound);
            }

            if (project.CreatedById != request.CurrentUserId)
            {
                return Result.Failure<TaskResponse>(ProjectErrors.UnauthorizedAccess);
            }

            var task = request.Adapt<ProjectTask>();

            await _taskRepository.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveAsync($"project-tasks-{request.ProjectId}", cancellationToken);

            var response = task.Adapt<TaskResponse>();

            return Result.Success(response);
        }
    }
}
