using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.Models.TicketModels;
using PlayNirvana.Bll.Services;

namespace PlayNirvana.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly TicketService ticketService;

        public TicketController(TicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        [HttpPost]
        public void CreateTicket(CreatTicketModel creatTicketModel)
        {
            this.ticketService.ValidateAndCreateTicket(creatTicketModel);
        }
    }
}
