using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rice.NuBank.Business.NuBank;
using Rice.NuBank.Domain.Authentication;

namespace Rice.NuBank.API.Controllers
{
    public class EventsController: Controller
    {
        private readonly INubankService _nubankService;

        public EventsController(INubankService nubankService)
        {
            _nubankService = nubankService;
        }
        
        [AllowAnonymous]
        [HttpGet("api/[controller]/")]
        public async Task<IActionResult> GetEvents(string cpf)
        {
            var events = await _nubankService.GetEvents(cpf);
            return Ok(events);
        }
        
        [AllowAnonymous]
        [HttpGet("api/[controller]/month")]
        public async Task<IActionResult> GetCurrentMonthEvents(string cpf)
        {
            var events = await _nubankService.GetEvents(cpf);
            return Ok(events);
        }
    }
}