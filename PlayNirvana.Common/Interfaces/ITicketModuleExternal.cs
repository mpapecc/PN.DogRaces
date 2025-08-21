using PlayNirvana.Common.Models;

namespace PlayNirvana.Common.Interfaces
{
    public interface ITicketModuleExternal
    {
        void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData);
    }
}
