using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Bll.Models;

namespace PlayNirvana.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : Controller
    {
        private readonly RoundRepository roundRepository;

        public RoundController(RoundRepository roundRepository)
        {
            this.roundRepository = roundRepository;
        }

        [HttpGet(nameof(GetActiveRounds))]
        public IEnumerable<RoundModel> GetActiveRounds()
        {
            return this.roundRepository.GetActiveRounds();
        }
    }
}
