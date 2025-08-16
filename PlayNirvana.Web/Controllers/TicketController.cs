using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.Models.TicketModels;
using PlayNirvana.Bll.Services;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly IPublishEndpoint publish;
        private readonly TicketService ticketService;

        public TicketController(
            IPublishEndpoint publish,
            TicketService ticketService)
        {
            this.publish = publish;
            this.ticketService = ticketService;
        }

        [HttpPost]
        public Task CreateTicket(CreateTicketModel creatTicketModel)
        {
            return this.publish.Publish(creatTicketModel);
        }

        [HttpGet]
        public IList<Ticket> GetTickets()
        {
            return this.ticketService.GetTickets();
        }
    }
}
