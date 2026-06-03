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
        public async Task<Result<IEnumerable<RoleResponse>>> GetAll([FromQuery] bool IncludeDisabled, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetAllRoles(IncludeDisabled), cancellationToken);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result<RoleDetailResponse>> GetById([FromRoute] long id, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetRoleById(id), cancellationToken);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<Result<RoleDetailResponse>> Add([FromBody] AddRole command)
        {
           return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<Result> Update(long id, [FromBody] UpdateRole command, CancellationToken cancellationToken)
        {
            command = command with { id = id };
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpPut("{id}/toggle-status")]
        [MapToApiVersion("1.0")]
        public async Task<Result> ToggleStatus(long id, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ToggleRoleStatus(id), cancellationToken);
        }
    }
}
