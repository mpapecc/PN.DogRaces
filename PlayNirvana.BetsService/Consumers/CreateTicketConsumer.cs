using MassTransit;
using PlayNirvana.Bll.Models.TicketModels;
using PlayNirvana.Bll.Services;

namespace PlayNirvana.BetsService.Consumers
{
    public class CreateTicketConsumer : IConsumer<CreateTicketModel>
    {
        private readonly TicketService ticketService;

        public CreateTicketConsumer(TicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        public Task Consume(ConsumeContext<CreateTicketModel> context)
        {
            this.ticketService.ValidateAndCreateTicket(context.Message);
            return Task.CompletedTask;
        }
    }
}
