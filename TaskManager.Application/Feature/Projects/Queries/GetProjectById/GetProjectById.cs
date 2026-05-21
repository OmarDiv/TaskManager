using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
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
            string cacheKey = $"project-{request.Id}";
            var response = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
                if (project == null) return null;
                
                return project.Adapt<ProjectResponse>();
            }, cancellationToken);

            if (response == null)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.NotFound);
            }

            // Ownership check
            if (response.CreatedById != request.CurrentUserId)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.UnauthorizedAccess);
            }

            return Result.Success(response);
        }
    }
}
