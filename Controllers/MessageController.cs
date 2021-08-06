using Hotvenues.Helpers;
using Hotvenues.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Controllers
{
    public class MessagesController : BaseApi<IMessageService, MessageDto>
    {
        public MessagesController(IMessageService service) : base(service) { }

        [HttpGet, Route("contacts")]
        public async Task<IActionResult> SearchContacts(string q)
        {
            try { return Ok(await Service.SearchContacts(q)); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpGet, Route("query")]
        public async Task<ActionResult<MessageDto>> Query([FromQuery]MessageFilter filter)
        {
            try { return Ok(await Service.Query(filter)); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpPost, Route("send")]
        public async Task<IActionResult> Send(MessageDto message)
        {
            try
            {
                var (success, response) = await Service.Send(message);
                if (success) return Ok(new { response });
                return BadRequest(response);
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpGet, Route("resend")]
        public async Task<IActionResult> Resend(long id)
        {
            try
            {
                var (success, response) = await Service.Resend(id);
                if (success) return Ok(new { response });
                return BadRequest(response);
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpPost, Route("sendbulk")]
        public async Task<IActionResult> SendBulk(BulkMessage message)
        {
            try
            {
                var (success, response) = await Service.BulkSend(message);
                if (success) return Ok(new { response });
                return BadRequest(response);
            }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpGet, Route("creditbalance")]
        public async Task<IActionResult> CreditBalance()
        {
            try { return Ok(await Service.CreditBalance()); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

        [HttpGet, Route("topup")]
        public async Task<IActionResult> TopUp()
        {
            try { return Ok(await Service.GetTopUpLink()); }
            catch (Exception ex) { return BadRequest(ExceptionHelper.ProcessException(ex)); }
        }

    }
}