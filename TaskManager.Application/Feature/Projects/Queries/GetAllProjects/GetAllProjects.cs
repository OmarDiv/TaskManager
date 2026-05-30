using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Application.Feature.Projects.Queries.GetAllProjects
{
    public record GetAllProjects(long UserId) : IRequest<Result<List<ProjectResponse>>>;

    public class GetAllProjectsHandler(
        IGenericRepository<Project> _projectRepository,
        ICacheService _cacheService
    ) : IRequestHandler<GetAllProjects, Result<List<ProjectResponse>>>
    {
        public async Task<Result<List<ProjectResponse>>> Handle(GetAllProjects request, CancellationToken cancellationToken)
        {
            var cacheKey = $"projects-{request.UserId}-{CultureInfo.CurrentCulture.Name}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var projects = await _projectRepository.AsQueryable()
                    .Include(p => p.NameSet).ThenInclude(ns => ns.Localization)
                    .Include(p => p.DescriptionSet).ThenInclude(ds => ds.Localization)
                    .Where(p => p.CreatedById == request.UserId)
                    .ToListAsync(cancellationToken);

                return projects.Adapt<List<ProjectResponse>>();
            }, cancellationToken);

            return Result.Success(response!);
        }
    }
}
