using Hotvenues.Helpers;
using Hotvenues.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AuthController(IUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _hostingEnvironment = environment;
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginParams model)
        {

            if (!ModelState.IsValid) return BadRequest(ExceptionHelper.ProcessException(ModelState));

            try { return Ok(await _userService.Authenticate(model)); }
            catch (Exception ex) { return Unauthorized(ex.Message); }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> UserProfile()
        {
            try { return Ok(await _userService.UserProfile(User.FindFirst("username")?.Value)); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateUserProfile(UserProfileDto profile)
        {
            try
            {
                profile.Username = User.FindFirst("username")?.Value;
                profile.RootPath = _hostingEnvironment.ContentRootPath;
                return Ok(await _userService.UpdateUserProfile(profile));
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            try
            {
                passwords.Username = User.FindFirst("username")?.Value;
                return Ok(await _userService.ChangePassword(passwords));
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

    }
}