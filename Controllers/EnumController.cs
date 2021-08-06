using Hotvenues.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Hotvenues.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController, Authorize]
    public class EnumsController : ControllerBase
    {
        private readonly IEnumService _enumService;

        public EnumsController(IEnumService enumService) => _enumService = enumService;

        public ActionResult GetList(string name)
        {
            try { return Ok(_enumService.GetList(name)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}