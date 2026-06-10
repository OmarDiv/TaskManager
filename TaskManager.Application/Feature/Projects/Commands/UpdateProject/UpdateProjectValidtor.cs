using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;
using TaskManager.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Application.Feature.Projects.Commands.UpdateProject
{
    public class UpdateProjectValidtor : AbstractValidator<UpdateProject>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public UpdateProjectValidtor(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ResultMessage.GreaterThan);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .MustContainArabicLocalization(ResultMessage.ArabicLanguageRequired);
            RuleForEach(x => x.Name).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ResultMessage.Required);
            RuleForEach(x => x.Description).SetValidator(new LocalizationDtoValidator());

            // 1. Verify project exists
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await _projectRepository.IsExist(p => p.Id == id, cancellationToken);
                })
                .WithMessage(ResultMessage.ProjectNotFound);

            // 2. Verify ownership
            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) =>
                {
                    var project = await _projectRepository.GetByIdAsync(command.Id, cancellationToken);
                    if (project == null) return true; // Let the existence rule handle it

                    return project.CreatedById == command.UserId;
                })
                .WithMessage(ResultMessage.ProjectUnauthorizedAccess);

            // 3. Verify duplicate name check (excluding this project)
            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) =>
                {
                    var requestNames = command.Name
                        .Select(x => (x.Value ?? "").Trim().ToLower())
                        .ToList();

                    var exists = await _projectRepository.IsExist(
                        p => p.CreatedById == command.UserId &&
                             p.Id != command.Id &&
                             p.NameSet.Localization.Any(l => requestNames.Contains(l.Value.ToLower())),
                        cancellationToken);

                    return !exists;
                })
                .WithMessage(ResultMessage.ProjectDuplicateName);
        }
    }
}
