using Microsoft.Extensions.Logging;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.CommonModule.Models;

namespace PlayNirvana.RoundModule.Integrations
{
    public interface ITicketModuleIntegration
    {
        void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData);
    }

    public class TicketModuleIntegration : ITicketModuleIntegration
    {
        private readonly ITicketModuleExternal ticketModuleExternal;
        private readonly ILogger<TicketModuleIntegration> logger;

        public TicketModuleIntegration(ITicketModuleExternal ticketModuleExternal, ILogger<TicketModuleIntegration> logger)
        {
            this.ticketModuleExternal = ticketModuleExternal;
            this.logger = logger;
        }

        public void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData)
        {
            this.logger.LogInformation($" {DateTime.UtcNow} : Bets round {roundBetsProcessData.RoundId} proccess started");

            this.ticketModuleExternal.ProcessRoundBets(roundBetsProcessData);
            this.logger.LogInformation($" {DateTime.UtcNow} : Bets round {roundBetsProcessData.RoundId} proccess finished");

        }
    }
}
