using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Application.Feature.Projects.Commands.UpdateProject
{
    public record UpdateProject(long Id, List<LocalizationDto> Name, List<LocalizationDto> Description, long UserId) : IRequest<Result<ProjectResponse>>;

    public class UpdateProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<UpdateProject, Result<ProjectResponse>>
    {
        public async Task<Result<ProjectResponse>> Handle(UpdateProject request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.AsQueryable()
                .Include(p => p.NameSet).ThenInclude(ns => ns.Localization)
                .Include(p => p.DescriptionSet).ThenInclude(ds => ds.Localization)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                
            if (project == null)
            {
                return ResultMessage.ProjectNotFound;
            }

            // Ownership check
            if (project.CreatedById != request.UserId)
            {
                return ResultMessage.ProjectUnauthorizedAccess;
            }

            // Check duplicate name (excluding this project)
            var requestNames = request.Name.Select(x => (x.Value ?? "").ToLower()).ToList();
            var exists = await _projectRepository.IsExist(
                p => p.CreatedById == request.UserId && p.Id != request.Id && p.NameSet.Localization.Any(l => requestNames.Contains(l.Value.ToLower())),
                cancellationToken
            );

            if (exists)
            {
                return ResultMessage.ProjectDuplicateName;
            }

            request.Adapt(project);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveByPrefixAsync($"project-{request.Id}-", cancellationToken);
            await _cacheService.RemoveByPrefixAsync($"projects-{request.UserId}-", cancellationToken);

            var response = project.Adapt<ProjectResponse>();

            return Result.Success(response);
        }
    }
}
