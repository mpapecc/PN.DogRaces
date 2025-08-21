using Microsoft.AspNetCore.Mvc;
using PlayNirvana.TicketModule.Application.Models;
using PlayNirvana.TicketModule.Application.Services;

namespace PlayNirvana.TicketModule.Presentation
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly TicketService ticketService;

        public TicketController(TicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        [HttpPost]
        public Task CreateTicket(CreateTicketModel creatTicketModel)
        {
            ticketService.ValidateAndCreateTicket(creatTicketModel);
            return Task.CompletedTask;
        }
    }
}
