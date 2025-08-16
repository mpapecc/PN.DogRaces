using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.Models.TicketModels;
using PlayNirvana.Bll.Services;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly IPublishEndpoint publish;
        private readonly TicketService ticketService;

        public TicketController(
            IPublishEndpoint publish,
            TicketService ticketService
            )
        {
            this.publish = publish;
            this.ticketService = ticketService;
        }

        [HttpPost]
        public Task CreateTicket(CreateTicketModel creatTicketModel)
        {
            return this.publish.Publish(creatTicketModel);
        }

        //method that will get tickets with pagination based on TicketStatus. if no argument is provided get without filtering

        [HttpGet(nameof(GetSuccessTickets))]
        public IEnumerable<TicketModel> GetSuccessTickets()
        {
            return this.ticketService.GetSuccessTickets();
        }

        [HttpGet(nameof(GetTickedDetails))]
        public TicketDetailsModel GetTickedDetails(int ticketId)
        {
            return this.ticketService.GetTicketDetails(ticketId);
        }
    }
}
