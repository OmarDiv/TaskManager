using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Projects.Commands.DeleteProject
{
    public record DeleteProject(long Id, long UserId) : IRequest<Result>;

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
                return ResultMessage.ProjectNotFound;
            }

            if (project.CreatedById != request.UserId)
            {
                return ResultMessage.ProjectUnauthorizedAccess;
            }

            await _projectRepository.DeleteAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveByPrefixAsync($"projects-{request.UserId}-", cancellationToken);
            await _cacheService.RemoveByPrefixAsync($"project-{request.Id}-", cancellationToken);
            await _cacheService.RemoveByPrefixAsync($"project-tasks-{request.Id}-", cancellationToken);

            return Result.Success();
        }
    }
}
