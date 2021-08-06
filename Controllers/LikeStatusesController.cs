using Hotvenues.Services;
using Hotvenues.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    public class LikeStatusesController : BaseApi<ILikeStatusService, LikeStatusDto>
    {
        public LikeStatusesController(ILikeStatusService service) : base(service) { }


        [HttpGet, Route("like")]
        public async Task<ActionResult<bool>> LikeStatus(long id)
        {
            try
            {
                var username = User.FindFirst("username")?.Value;
                return Ok(await Service.LikeStatus(id, username));
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }
    }
}