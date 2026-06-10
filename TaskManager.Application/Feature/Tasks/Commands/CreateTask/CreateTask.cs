using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Commands.CreateTask
{
    public record CreateTask(
        List<LocalizationDto> Title,
        List<LocalizationDto> Description,
        Status Status,
        DateTime? DueDate,
        Priority Priority,
        long ProjectId,
        long UserId
    ) : IRequest<Result<TaskResponse>>;

    public class CreateTaskHandler(
        IGenericRepository<ProjectTask> _taskRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<CreateTask, Result<TaskResponse>>
    {
        public async Task<Result<TaskResponse>> Handle(CreateTask request, CancellationToken cancellationToken)
        {
            var task = request.Adapt<ProjectTask>();

            await _taskRepository.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveByPrefixAsync($"project-tasks-{request.ProjectId}-", cancellationToken);

            var response = task.Adapt<TaskResponse>();

            return Result.Success(response);
        }
    }
}
