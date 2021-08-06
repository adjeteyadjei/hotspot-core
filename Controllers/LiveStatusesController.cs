using Hotvenues.Services;
using Hotvenues.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    public class LiveStatusesController : BaseApi<ILiveStatusService, LiveStatusDto>
    {
        private IWebHostEnvironment _environment;
        public LiveStatusesController(ILiveStatusService service, IWebHostEnvironment environment) : base(service)
        {
            _environment = environment;
        }
         
        [HttpGet, Route("query")]
        public async Task<IActionResult> Query([FromQuery]LiveStatusFilter filter)
        {
            try { return Ok(await Service.Query(filter)); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        public override Task<ActionResult> Create(LiveStatusDto model)
        {
            model.RootPath = _environment.ContentRootPath;
            model.Username = GetUsername();
            return base.Create(model);
        }

        [HttpGet, Route("topPosts")]
        public async Task<IActionResult> TopPosts()
        {
            try { return Ok(await Service.TopPosts()); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

    }
}