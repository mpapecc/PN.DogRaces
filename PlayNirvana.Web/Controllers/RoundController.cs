using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.Models;
using PlayNirvana.Bll.Services;

namespace PlayNirvana.Web.Controllers
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
        public IEnumerable<RoundModel> GetActiveRounds()
        {
            return this.roundService.GetActiveRounds();
        }
    }
}
