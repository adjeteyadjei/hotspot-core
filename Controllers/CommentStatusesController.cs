using Hotvenues.Services;
using Hotvenues.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    public class CommentStatusesController : BaseApi<ICommentStatusService, CommentStatusDto>
    {
        public CommentStatusesController(ICommentStatusService service) : base(service) { }

        [HttpPost, Route("comment")]
        public async Task<IActionResult> CommentStatus(CommentStatusDto record)
        {
            try
            {
                var username = User.FindFirst("username")?.Value;
                return Ok(await Service.CommentStatus(record.Id, username, record.Text));
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpGet, Route("getComment")]
        public async Task<IActionResult> Comments(long id)
        {
            try
            {
                var username = User.FindFirst("username")?.Value;
                return Ok(await Service.Comments(id));
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }
    }
}