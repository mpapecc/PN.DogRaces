using Microsoft.AspNetCore.Mvc;
using PlayNirvana.TicketModule.Application.Models;
using PlayNirvana.TicketModule.Application.Services;
using PlayNirvana.TicketModule.Common.Enums;

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

        [HttpPost(nameof(CheckTicketStatus))]
        public TicketStatus CheckTicketStatus(int ticketId)
        {
            return this.ticketService.CheckTicketStatus(ticketId);
        }

        [HttpPost]
        public void CreateTicket(CreateTicketCommand creatTicketModel)
        {
            this.ticketService.ValidateAndCreateTicket(creatTicketModel);
        }
    }
}
