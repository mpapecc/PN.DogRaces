using System;
using Microsoft.Extensions.Logging;
using PlayNirvana.CommonModule.Interfaces;

namespace PlayNirvana.RoundModule.Integrations
{
    public interface ITicketModuleIntegration
    {
        void ProcessRoundBets(int roundId);
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

        public void ProcessRoundBets(int roundId)
        {
            
            ThreadPool.QueueUserWorkItem(
                    _ => { this.logger.LogInformation($" {DateTime.UtcNow} : Bets round {roundId} proccess started"); });
            this.ticketModuleExternal.ProcessRoundBets(roundId);
            
            ThreadPool.QueueUserWorkItem(
                    _ => { this.logger.LogInformation($" {DateTime.UtcNow} : Bets round {roundId} proccess finished"); });
        }
    }
}
