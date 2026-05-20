using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Api.DTOs.Projects;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Commands.CreateProject;
using TaskManager.Application.Feature.Projects.Commands.UpdateProject;
using TaskManager.Application.Feature.Projects.Commands.DeleteProject;
using TaskManager.Application.Feature.Projects.Queries.GetAllProjects;
using TaskManager.Application.Feature.Projects.Queries.GetProjectById;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new CreateProject(dto.Name, dto.Description, userId);
            var result = await _mediator.Send(command, cancellationToken);
            
            return result.IsSuccess
                ? result.AsCreatedResult(nameof(GetById), new { id = result.Value.Id })
                : result.AsActionResult();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetAllProjects(userId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.AsActionResult();
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        public async Task<ActionResult<ProjectResponse>> GetById([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetProjectById(id, userId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.AsActionResult();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectResponse>> Update([FromRoute] long id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new UpdateProject(id, dto.Name, dto.Description, userId);
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteProject(id, userId);
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsNoContentResult();
        }
    }
}
