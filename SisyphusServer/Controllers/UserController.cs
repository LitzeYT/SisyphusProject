using Microsoft.AspNetCore.Mvc;

using SisyphusServer.Commands;
using SisyphusServer.Database.Entities;
using SisyphusServer.Extensions.Api.Controllers;
using SisyphusServer.Queries;

namespace SisyphusServer.Controllers {
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserController : ApiControllerBase {
        [HttpGet("ranking")]
        [ProducesResponseType(typeof(List<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UserInfo>>> GetRanking() {
            return await Sender.Send(new GetRankingQuery());
        }

        [HttpPost("{userid}/{points:long}")]
        public async Task<ActionResult<UserInfo>> AddOrUpdateUser(string userid, long points) {
            return await Sender.Send(new AddOrUpdateUserCommand { 
                UserId = userid,
                Points = points
            });
        }
    }
}
