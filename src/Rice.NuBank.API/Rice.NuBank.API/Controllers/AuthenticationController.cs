using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rice.NuBank.Business.NuBank;
using Rice.NuBank.Domain.Authentication;

namespace Rice.NuBank.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly INubankService _nubankService;

        public AuthenticationController(INubankService nubankService)
        {
            _nubankService = nubankService;
        }
        
        [AllowAnonymous]
        [HttpPost("api/[controller]/login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCredentials login)
        {
            var qrCode = await _nubankService.Login(login);
            return Ok(qrCode);
        }
        
        [AllowAnonymous]
        [HttpPost("api/[controller]/qr-code")]
        public async Task<IActionResult> ValidateQrCode(
            [FromBody] QrCode qrCode)
        {
            await _nubankService.ValidateQrCode(qrCode); 
            return Ok(qrCode);
        }
    }
}