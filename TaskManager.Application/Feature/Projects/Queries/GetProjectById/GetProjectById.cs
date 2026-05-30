using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Application.Feature.Projects.Queries.GetProjectById
{
    public record GetProjectById(long Id, long CurrentUserId) : IRequest<Result<ProjectResponse>>;

    public class GetProjectByIdHandler(
        IGenericRepository<Project> _projectRepository,
        ICacheService _cacheService
    ) : IRequestHandler<GetProjectById, Result<ProjectResponse>>
    {
        public async Task<Result<ProjectResponse>> Handle(GetProjectById request, CancellationToken cancellationToken)
        {
            string cacheKey = $"project-{request.Id}-{CultureInfo.CurrentCulture.Name}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var project = await _projectRepository.AsQueryable()
                    .Include(p => p.NameSet).ThenInclude(ns => ns.Localization)
                    .Include(p => p.DescriptionSet).ThenInclude(ds => ds.Localization)
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                if (project == null) return null;
                
                return project.Adapt<ProjectResponse>();
            }, cancellationToken);

            if (response == null)
            {
                return ResultMessage.ProjectNotFound;
            }

            // Ownership check
            if (response.CreatedById != request.CurrentUserId)
            {
                return ResultMessage.ProjectUnauthorizedAccess;
            }

            return Result.Success(response);
        }
    }
}
