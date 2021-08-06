using Hotvenues.Services;
using Hotvenues.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    public class UpcomingEventsController : BaseApi<IUpcomingEventService, UpcomingEventDto>
    {
        private IWebHostEnvironment _environment;
        public UpcomingEventsController(IUpcomingEventService service, IWebHostEnvironment environment) : base(service)
        {
            _environment = environment;
        }

        [HttpGet, Route("query")]
        public async Task<IActionResult> Query([FromQuery]UpcomingEventFilter filter)
        {
            try { return Ok(await Service.Query(filter)); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        public override Task<ActionResult> Create(UpcomingEventDto model)
        {
            model.RootPath = _environment.ContentRootPath;
            model.Username = GetUsername();
            return base.Create(model);
        }
    }
}