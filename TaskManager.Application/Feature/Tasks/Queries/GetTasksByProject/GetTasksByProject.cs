using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Tasks.Queries.GetTasksByProject
{
    public record GetTasksByProject(long ProjectId, long CurrentUserId) : IRequest<Result<IEnumerable<TaskResponse>>>;

    public class GetTasksByProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IGenericRepository<ProjectTask> _taskRepository,
        ICacheService _cacheService
    ) : IRequestHandler<GetTasksByProject, Result<IEnumerable<TaskResponse>>>
    {
        public async Task<Result<IEnumerable<TaskResponse>>> Handle(GetTasksByProject request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            if (project == null)
            {
                return Result.Failure<IEnumerable<TaskResponse>>(ProjectErrors.NotFound);
            }

            if (project.CreatedById != request.CurrentUserId)
            {
                return Result.Failure<IEnumerable<TaskResponse>>(ProjectErrors.UnauthorizedAccess);
            }

            string cacheKey = $"project-tasks-{request.ProjectId}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var tasks = await _taskRepository.GetListByCriteria(
                    t => t.ProjectId == request.ProjectId,
                    cancellationToken
                );

                return tasks.Adapt<List<TaskResponse>>();
            }, cancellationToken);

            return Result.Success<IEnumerable<TaskResponse>>(response ?? []);
        }
    }
}
