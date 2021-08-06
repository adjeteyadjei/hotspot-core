using Hotvenues.Helpers;
using Hotvenues.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILiveStatusService _service;
        private readonly IUpcomingEventService _eventService;
        public MediaController(IWebHostEnvironment environment, ILiveStatusService service, IUpcomingEventService eventService)
        {
            _hostingEnvironment = environment;
            _service = service;
            _eventService = eventService;
        }

        public async Task<IActionResult> Get(string file)
        {
            try
            {
                var ext = "";
                try { ext = await _service.GetMediaExtension(file); } catch(Exception ex) { ext = await _eventService.GetMediaExtension(file);  }

                var fileStream = new MediaHelper(_hostingEnvironment.ContentRootPath).GetMedia(file);
                return new FileStreamResult(fileStream, ext);
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionHelper.ProcessException(ex));
            }

        }
    }
}