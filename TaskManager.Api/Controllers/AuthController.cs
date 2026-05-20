using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Commands.Login;
using TaskManager.Application.Feature.Auth.Commands.GetRefreshToken;
using TaskManager.Application.Feature.Auth.Commands.RevokeRefreshToken;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Users.Commands.RegisterUser;
using TaskManager.Api.Extention;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterUser request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.AsNoContentResult();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUser request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.AsActionResult();
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] GetRefrshToken request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.AsActionResult();
        }

        [HttpPost("Revoke")]
        public async Task<ActionResult<Result>> RevokeRefreshToken([FromBody] RevokeRefreshToken request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.AsNoContentResult();
        }
    }
}
