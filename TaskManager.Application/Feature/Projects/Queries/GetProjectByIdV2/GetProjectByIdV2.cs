using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Application.Feature.Projects.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Projects.Queries.GetProjectByIdV2
{
    public record GetProjectByIdV2(long Id, long UserId) : IRequest<Result<ProjectResponseV2>>;

    public class GetProjectByIdV2Handler(
        IGenericRepository<Project> _projectRepository
    ) : IRequestHandler<GetProjectByIdV2, Result<ProjectResponseV2>>
    {
        public async Task<Result<ProjectResponseV2>> Handle(GetProjectByIdV2 request, CancellationToken cancellationToken)
        {
            // V2 Logic: Might include more data, like task count
            var project = await _projectRepository.GetById(
                request.Id,
                query => query.Include(p => p.Tasks),
                cancellationToken
            );

            if (project == null || project.CreatedById != request.UserId)
            {
                return Result.Failure<ProjectResponseV2>(ProjectErrors.NotFound);
            }

            var response = new ProjectResponseV2(
                project.Id,
                project.Name,
                project.Description,
                project.Tasks.Count,
                project.CreatedById,
                DateTime.UtcNow // In real case, use project.CreatedAt if exists
            );

            return Result.Success(response);
        }
    }
}
