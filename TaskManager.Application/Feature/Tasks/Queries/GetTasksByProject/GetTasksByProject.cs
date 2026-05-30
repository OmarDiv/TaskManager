using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Tasks.Queries.GetTasksByProject
{
    public record GetTasksByProject(long ProjectId, long UserId) : IRequest<Result<IEnumerable<TaskResponse>>>;

    public class GetTasksByProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IGenericRepository<ProjectTask> _taskRepository,
        ICacheService _cacheService
    ) : IRequestHandler<GetTasksByProject, Result<IEnumerable<TaskResponse>>>
    {
        public async Task<Result<IEnumerable<TaskResponse>>> Handle(GetTasksByProject request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the current user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            if (project == null)
            {
                return ResultMessage.ProjectNotFound;
            }

            if (project.CreatedById != request.UserId)
            {
                return ResultMessage.ProjectUnauthorizedAccess;
            }

            var cacheKey = $"project-tasks-{request.ProjectId}-{CultureInfo.CurrentCulture.Name}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var tasks = await _taskRepository.AsQueryable()
                    .Include(t => t.TitleSet).ThenInclude(ts => ts.Localization)
                    .Include(t => t.DescriptionSet).ThenInclude(ds => ds.Localization)
                    .Where(t => t.ProjectId == request.ProjectId)
                    .ToListAsync(cancellationToken);

                return tasks.Adapt<List<TaskResponse>>();
            }, cancellationToken);

            return Result.Success<IEnumerable<TaskResponse>>(response ?? []);
        }
    }
}
