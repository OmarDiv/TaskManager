using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Commands.UpdateTask
{
    public record UpdateTask(
        long Id,
        List<LocalizationDto> Title,
        List<LocalizationDto> Description,
        Status Status,
        DateTime? DueDate,
        Priority Priority,
        long UserId
    ) : IRequest<Result<TaskResponse>>;

    public class UpdateTaskHandler(
        IGenericRepository<ProjectTask> _taskRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<UpdateTask, Result<TaskResponse>>
    {
        public async Task<Result<TaskResponse>> Handle(UpdateTask request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.AsQueryable()
                .Include(t => t.Project)
                .Include(t => t.TitleSet).ThenInclude(ts => ts.Localization)
                .Include(t => t.DescriptionSet).ThenInclude(ds => ds.Localization)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            request.Adapt(task!);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveByPrefixAsync($"project-tasks-{task!.ProjectId}-", cancellationToken);

            var response = task.Adapt<TaskResponse>();

            return Result.Success(response);
        }
    }
}
