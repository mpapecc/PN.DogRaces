using PlayNirvana.CommonModule.Models;

namespace PlayNirvana.CommonModule.Interfaces
{
    public interface ITicketModuleExternal
    {
        void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData);
    }
}
