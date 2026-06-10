using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;
using TaskManager.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Application.Feature.Tasks.Commands.UpdateTask
{
    public class UpdateTaskValidtor : AbstractValidator<UpdateTask>
    {
        private readonly IGenericRepository<ProjectTask> _taskRepository;

        public UpdateTaskValidtor(IGenericRepository<ProjectTask> taskRepository)
        {
            _taskRepository = taskRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ResultMessage.GreaterThan);

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

            // 1. Verify task exists
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await _taskRepository.IsExist(t => t.Id == id, cancellationToken);
                })
                .WithMessage(ResultMessage.TaskNotFound);

            // 2. Verify task ownership
            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) =>
                {
                    var task = await _taskRepository.GetById(command.Id, q => q.Include(t => t.Project), cancellationToken);
                    if (task == null) return true; // Let the existence rule handle it

                    return task.Project.CreatedById == command.UserId;
                })
                .WithMessage(ResultMessage.TaskUnauthorizedAccess);
        }
    }
}
