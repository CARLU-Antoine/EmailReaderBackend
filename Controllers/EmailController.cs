using Microsoft.AspNetCore.Mvc;
using EmailReaderBackend.Services;
using System.Threading.Tasks;

namespace EmailReaderBackend.Controllers
{
    [ApiController]
    [Route("api/emails")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmails()
        {
            var emails = await _emailService.GetEmailsAsync();
            return Ok(emails);
        }
    }
}
