using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Commands.Login;
using TaskManager.Application.Feature.Auth.Commands.GetRefreshToken;
using TaskManager.Application.Feature.Auth.Commands.RevokeRefreshToken;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Users.Commands.RegisterUser;

namespace TaskManager.Api.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("Register")]
        [MapToApiVersion("1.0")]
        public async Task<Result> Register([FromBody] RegisterUser request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("Login")]
        [MapToApiVersion("1.0")]
        public async Task<Result<AuthResponse>> Login([FromBody] LoginUser request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("Refresh")]
        [MapToApiVersion("1.0")]
        public async Task<Result<AuthResponse>> RefreshToken([FromBody] GetRefrshToken request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("Revoke")]
        [MapToApiVersion("1.0")]
        public async Task<Result> RevokeRefreshToken([FromBody] RevokeRefreshToken request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
