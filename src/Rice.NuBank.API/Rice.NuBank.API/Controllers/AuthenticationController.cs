using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rice.NuBank.API.Attributes;
using Rice.NuBank.Business.NuBank;
using Rice.NuBank.Domain.Authentication;
using Rice.SDK.Utils;

namespace Rice.NuBank.API.Controllers
{
    [ApiExceptionFilter]
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
            return Ok(await _nubankService.Login(login));
        }

        [Authorize]
        [HttpGet("api/[controller]/qr-code")]
        public async Task<IActionResult> GetQrCode()
        {
            return Ok(await _nubankService.GetQrCode(User));
        }
        
        [Authorize]
        [HttpPost("api/[controller]/qr-code")]
        public async Task<IActionResult> ValidateQrCode()
        {
            await _nubankService.ValidateQrCode(User); 
            return Ok();
        }
    }
}