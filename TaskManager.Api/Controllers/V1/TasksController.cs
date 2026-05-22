using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Tasks.Commands.CreateTask;
using TaskManager.Application.Feature.Tasks.Commands.UpdateTaskStatus;
using TaskManager.Application.Feature.Tasks.Commands.DeleteTask;
using TaskManager.Application.Feature.Tasks.Queries.GetTasksByProject;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTask command, CancellationToken cancellationToken)
        {
            command = command with { UserId = User.GetUserId() };

            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetTasksByProject), new { projectId = result.Value.ProjectId }, result.Value)
                : result.AsActionResult();
        }

        [HttpGet("project/{projectId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasksByProject([FromRoute] long projectId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetTasksByProject(projectId, userId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.AsActionResult();
        }

        [HttpPut("{id}/status")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<TaskResponse>> UpdateStatus([FromRoute] long id, [FromBody] UpdateTaskStatus command, CancellationToken cancellationToken)
        {
            command = command with { Id = id, UserId = User.GetUserId() };
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsActionResult();
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Result>> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteTask(id, userId);
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsNoContentResult();
        }
    }
}
