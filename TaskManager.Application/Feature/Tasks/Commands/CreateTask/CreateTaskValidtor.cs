using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;
using TaskManager.Application.Common.Interfaces.Repositories;

namespace TaskManager.Application.Feature.Tasks.Commands.CreateTask
{
    public class CreateTaskValidtor : AbstractValidator<CreateTask>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public CreateTaskValidtor(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .MustContainArabicLocalization(ResultMessage.ArabicLanguageRequired);
            RuleForEach(x => x.Title).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ResultMessage.Required);
            RuleForEach(x => x.Description).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid task priority.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Invalid project ID.");

            // 1. Verify project exists
            RuleFor(x => x.ProjectId)
                .MustAsync(async (projectId, cancellationToken) =>
                {
                    return await _projectRepository.IsExist(p => p.Id == projectId, cancellationToken);
                })
                .WithMessage(ResultMessage.ProjectNotFound);

            // 2. Verify project ownership
            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) =>
                {
                    var projectExists = await _projectRepository.IsExist(p => p.Id == command.ProjectId, cancellationToken);
                    if (!projectExists) return true; // Let the existence rule handle it

                    return await _projectRepository.IsExist(
                        p => p.Id == command.ProjectId && p.CreatedById == command.UserId,
                        cancellationToken);
                })
                .WithMessage(ResultMessage.ProjectUnauthorizedAccess);
        }
    }
}
