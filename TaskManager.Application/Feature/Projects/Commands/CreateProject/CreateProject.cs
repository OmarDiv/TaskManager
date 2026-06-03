using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Feature.Projects.Responses;
using TaskManager.Application.Common.Localizations;

namespace TaskManager.Application.Feature.Projects.Commands.CreateProject
{
    public record CreateProject(
        List<LocalizationDto> Name, List<LocalizationDto> Description, long UserId) : IRequest<Result<ProjectResponse>>;

    public class CreateProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<CreateProject, Result<ProjectResponse>>
    {
        public async Task<Result<ProjectResponse>> Handle(CreateProject request, CancellationToken cancellationToken)
        {
            var requestNames = request.Name.Select(x => (x.Value ?? "").ToLower()).ToList();
            var exists = await _projectRepository.IsExist(
                p => p.CreatedById == request.UserId && p.NameSet.Localization.Any(l => requestNames.Contains(l.Value.ToLower())),
                cancellationToken
            );
            if (exists)
            {
                return ResultMessage.ProjectDuplicateName;
            }

            var project = request.Adapt<Project>();

            await _projectRepository.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"projects-{request.UserId}-", cancellationToken);

            // Map response using scanned Mapster configuration
            var response = project.Adapt<ProjectResponse>();

            return Result.Success(response);
        }
    }
}
