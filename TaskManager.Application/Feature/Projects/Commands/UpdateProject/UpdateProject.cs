using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Application.Feature.Projects.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Feature.Projects.Commands.UpdateProject
{
    public record UpdateProject(long Id, string Name, string Description, long CurrentUserId) : IRequest<Result<ProjectResponse>>;

    public class UpdateProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<UpdateProject, Result<ProjectResponse>>
    {
        public async Task<Result<ProjectResponse>> Handle(UpdateProject request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (project == null)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.NotFound);
            }

            // Ownership check
            if (project.CreatedById != request.CurrentUserId)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.UnauthorizedAccess);
            }

            // Check duplicate name (excluding this project)
            var exists = await _projectRepository.IsExist(
                p => p.CreatedById == request.CurrentUserId && p.Name.ToLower() == request.Name.ToLower() && p.Id != request.Id,
                cancellationToken
            );

            if (exists)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.DuplicateName);
            }

            project.Name = request.Name;
            project.Description = request.Description;

            await _projectRepository.UpdateAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate Caches
            await _cacheService.RemoveAsync($"projects-user-{request.CurrentUserId}", cancellationToken);
            await _cacheService.RemoveAsync($"project-{request.Id}", cancellationToken);

            var response = project.Adapt<ProjectResponse>();

            return Result.Success(response);
        }
    }
}
