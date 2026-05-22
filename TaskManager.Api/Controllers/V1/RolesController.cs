using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Roles.Commands.AddRole;
using TaskManager.Application.Feature.Roles.Commands.ToggleRoleStatus;
using TaskManager.Application.Feature.Roles.Commands.UpdateRole;
using TaskManager.Application.Feature.Roles.Queries.GetAllRoles;
using TaskManager.Application.Feature.Roles.Queries.GetRoleById;
using TaskManager.Application.Feature.Roles.Responses;

namespace TaskManager.Api.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetAll([FromQuery] bool IncludeDisabled, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllRoles(IncludeDisabled), cancellationToken);
            return result.AsActionResult();
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<RoleDetailResponse>> GetById([FromRoute] long id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRoleById(id), cancellationToken);
            return result.AsActionResult();
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<RoleDetailResponse>> Add([FromBody] AddRole command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? result.AsCreatedResult(nameof(GetById), new { id = result.Value.Id })
                : result.AsActionResult();
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Result>> Update(long id, [FromBody] UpdateRole command, CancellationToken cancellationToken)
        {
            command = command with { id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.AsNoContentResult();
        }

        [HttpPut("{id}/toggle-status")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Result>> ToggleStatus(long id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ToggleRoleStatus(id), cancellationToken);
            return result.AsNoContentResult();
        }
    }
}
