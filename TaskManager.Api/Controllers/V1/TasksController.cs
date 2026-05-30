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
using TaskManager.Application.Feature.Tasks.Commands.UpdateTask;
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
        public async Task<Result<TaskResponse>> Create([FromBody] CreateTask command, CancellationToken cancellationToken)
        {
            command = command with { UserId = User.GetUserId() };

            return await _mediator.Send(command, cancellationToken);
        }

        [HttpGet("project/{projectId}")]
        [MapToApiVersion("1.0")]
        public async Task<Result<IEnumerable<TaskResponse>>> GetTasksByProject([FromRoute] long projectId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetTasksByProject(projectId, userId);
            return await _mediator.Send(query, cancellationToken);
        }

        [HttpPut("{id}/status")]
        [MapToApiVersion("1.0")]
        public async Task<Result<TaskResponse>> UpdateStatus([FromRoute] long id, [FromBody] UpdateTaskStatus command, CancellationToken cancellationToken)
        {
            command = command with { Id = id, UserId = User.GetUserId() };
            return  await _mediator.Send(command, cancellationToken);
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result<TaskResponse>> Update([FromRoute] long id, [FromBody] UpdateTask command, CancellationToken cancellationToken)
        {
            command = command with { Id = id, UserId = User.GetUserId() };
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteTask(id, userId);
           return await _mediator.Send(command, cancellationToken);
        }
    }
}
