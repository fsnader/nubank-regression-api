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
    public class EventsController: Controller
    {
        private readonly INubankService _nubankService;

        public EventsController(INubankService nubankService)
        {
            _nubankService = nubankService;
        }
        
        [Authorize]
        [HttpGet("api/[controller]/")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _nubankService.GetEvents(User.GetUserName());
            return Ok(events);
        }
        
        [Authorize]
        [HttpGet("api/[controller]/month")]
        public async Task<IActionResult> GetCurrentMonthEvents(string cpf)
        {
            var events = await _nubankService.GetCurrentMonthSummary(User.GetUserName());
            return Ok(events);
        }
    }
}