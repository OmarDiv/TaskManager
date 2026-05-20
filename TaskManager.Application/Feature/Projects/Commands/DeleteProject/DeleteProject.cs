using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Projects.Commands.DeleteProject
{
    public record DeleteProject(long Id, long CurrentUserId) : IRequest<Result>;

    public class DeleteProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<DeleteProject, Result>
    {
        public async Task<Result> Handle(DeleteProject request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (project == null)
            {
                return Result.Failure(ProjectErrors.NotFound);
            }

            // Ownership check
            if (project.CreatedById != request.CurrentUserId)
            {
                return Result.Failure(ProjectErrors.UnauthorizedAccess);
            }

            await _projectRepository.DeleteAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveAsync($"projects-user-{request.CurrentUserId}", cancellationToken);
            await _cacheService.RemoveAsync($"project-{request.Id}", cancellationToken);
            await _cacheService.RemoveAsync($"project-tasks-{request.Id}", cancellationToken);

            return Result.Success();
        }
    }
}
