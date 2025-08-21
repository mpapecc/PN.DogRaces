using PlayNirvana.Common.Interfaces;
using PlayNirvana.Common.Models;
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

        public void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData)
        {
            this.betService.ProcessRoundBets(roundBetsProcessData);
        }
    }
}
