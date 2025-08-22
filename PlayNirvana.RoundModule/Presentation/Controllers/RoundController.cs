using Microsoft.AspNetCore.Mvc;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : Controller
    {
        private readonly RoundService roundService;

        public RoundController(RoundService roundService)
        {
            this.roundService = roundService;
        }

        [HttpGet(nameof(GetActiveRounds))]
        public IEnumerable<RoundDto> GetActiveRounds()
        {
            return roundService.GetActiveRounds();
        }
    }
}
