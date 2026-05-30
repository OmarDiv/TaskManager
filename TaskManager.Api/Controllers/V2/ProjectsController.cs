using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Api.Extention;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Projects.Queries.GetProjectByIdV2;
using TaskManager.Application.Feature.Projects.Responses;

namespace TaskManager.Api.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<Result<ProjectResponseV2>> GetById([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var query = new GetProjectByIdV2(id, userId);
            return await _mediator.Send(query, cancellationToken);
        }
        
        // Other methods would be here, mapped to 2.0
    }
}
