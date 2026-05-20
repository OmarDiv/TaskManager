using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Tasks.Commands.CreateTask;
using TaskManager.Application.Feature.Tasks.Commands.UpdateTaskStatus;
using TaskManager.Application.Feature.Tasks.Commands.DeleteTask;
using TaskManager.Application.Feature.Tasks.Queries.GetTasksByProject;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Enums;
namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new CreateTask(
                dto.Title,
                dto.Description,
                dto.Status,
                dto.DueDate,
                dto.Priority,
                dto.ProjectId,
                userId
            );

            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetTasksByProject), new { projectId = result.Value.ProjectId }, result.Value)
                : result.AsActionResult();
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasksByProject([FromRoute] long projectId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetTasksByProject(projectId, userId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.AsActionResult();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<TaskResponse>> UpdateStatus([FromRoute] long id, [FromBody] UpdateTaskStatusDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new UpdateTaskStatus(id, dto.Status, userId);
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteTask(id, userId);
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsNoContentResult();
        }
    }

    public record CreateTaskDto(
        string Title,
        string Description,
        Status Status,
        DateTime? DueDate,
        Priority Priority,
        long ProjectId
    );

    public record UpdateTaskStatusDto(Status Status);
}
