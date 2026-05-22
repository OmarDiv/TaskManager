using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Projects.Queries.GetAllProjects
{
    public record GetAllProjects(long UserId) : IRequest<Result<IEnumerable<ProjectResponse>>>;

    public class GetAllProjectsHandler(
        IGenericRepository<Project> _projectRepository,
        ICacheService _cacheService
    ) : IRequestHandler<GetAllProjects, Result<IEnumerable<ProjectResponse>>>
    {
        public async Task<Result<IEnumerable<ProjectResponse>>> Handle(GetAllProjects request, CancellationToken cancellationToken)
        {
            var cacheKey = $"projects-user-{request.UserId}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var projects = await _projectRepository.GetListByCriteria(
                    p => p.CreatedById == request.UserId,
                    cancellationToken
                );

                return projects.Adapt<List<ProjectResponse>>();
            }, cancellationToken);

            return Result.Success<IEnumerable<ProjectResponse>>(response ?? []);
        }
    }
}
