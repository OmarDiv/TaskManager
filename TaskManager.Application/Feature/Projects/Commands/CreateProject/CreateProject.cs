using Bogus.DataSets;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Feature.Projects.Errors;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Application.Feature.Projects.Commands.CreateProject
{
    public record CreateProject(
        string Name, string Description, long UserId) : IRequest<Result<ProjectResponse>>;

    public class CreateProjectHandler(
        IGenericRepository<Project> _projectRepository,
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
    ) : IRequestHandler<CreateProject, Result<ProjectResponse>>
    {
        public async Task<Result<ProjectResponse>> Handle(CreateProject request, CancellationToken cancellationToken)
        {
            //var exists = await _projectRepository.IsExist(
            //    p => p.CreatedById == request.UserId && p.Name.ToLower() == request.Name.ToLower(),
            //    cancellationToken
            //);
            //"الجزء ده كله المفروض يكون في Fluent Validation لكن قولت محاولتش اصعب الامور"
            // الطبيعي ان اليوزر مينفعش يوصل هنا غير لما نعمل تشيك علي كل الحاجات دي من ال FLUENT Validation 
            var exists = await _projectRepository.IsExist(
                p => p.CreatedById == request.UserId,
                cancellationToken
            );
            if (exists)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.DuplicateName);
            }

            var project = new Project
            {
                //Name = request.Name,
                //Description = request.Description,
                CreatedById = request.UserId
            };

            await _projectRepository.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"projects-user-{request.UserId}", cancellationToken);

            var response = project.Adapt<ProjectResponse>();

            return Result.Success(response);
        }
    }
}
