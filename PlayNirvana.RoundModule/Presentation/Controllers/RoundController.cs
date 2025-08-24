using Microsoft.AspNetCore.Mvc;
using PlayNirvana.RoundModule.Application;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : Controller
    {
        private readonly RoundService roundService;
        private readonly ActiveRoundCache activeRoundCache;

        public RoundController(
            RoundService roundService,
            ActiveRoundCache activeRoundCache)
        {
            this.roundService = roundService;
            this.activeRoundCache = activeRoundCache;
        }

        [HttpGet(nameof(GetActiveRounds))]
        public IEnumerable<RoundDto> GetActiveRounds()
        {
            return this.activeRoundCache.ToList();
        }
    }
}
