using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PlayNirvana.Bll.Models.TicketModels;

namespace PlayNirvana.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly IPublishEndpoint publish;

        public TicketController(IPublishEndpoint publish)
        {
            this.publish = publish;
        }

        [HttpPost]
        public Task CreateTicket(CreateTicketModel creatTicketModel)
        {
            return this.publish.Publish(creatTicketModel);
        }
    }
}
