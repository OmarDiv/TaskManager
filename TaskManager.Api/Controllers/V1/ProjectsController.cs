using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Commands.CreateProject;
using TaskManager.Application.Feature.Projects.Commands.UpdateProject;
using TaskManager.Application.Feature.Projects.Commands.DeleteProject;
using TaskManager.Application.Feature.Projects.Queries.GetAllProjects;
using TaskManager.Application.Feature.Projects.Queries.GetProjectById;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Api.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<Result<ProjectResponse>> Create([FromBody] CreateProject command, CancellationToken cancellationToken)
        {
            command = command with { UserId = User.GetUserId() };
           return await _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<Result<List<ProjectResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetAllProjects(userId);
            return await _mediator.Send(query, cancellationToken);
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        [MapToApiVersion("1.0")]
        public async Task<Result<ProjectResponse>> GetById([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetProjectById(id, userId);
            return await _mediator.Send(query, cancellationToken);
        }   

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result<ProjectResponse>> Update([FromRoute] long id, [FromBody] UpdateProject command, CancellationToken cancellationToken)
        {
            command = command with { Id = id, UserId = User.GetUserId() };
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteProject(id, userId);
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
