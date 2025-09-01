using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.TicketModule.Application.Services;

namespace PlayNirvana.TicketModule.External
{
    public class TicketModuleExternal : ITicketModuleExternal
    {
        private readonly BetService betService;

        public TicketModuleExternal(BetService betService)
        {
            this.betService = betService;
        }

        public void ProcessRoundBets(int roundId)
        {
            this.betService.ProcessRoundBets(roundId);
        }
    }
}
