using PlayNirvana.CommonModule.Interfaces;

namespace PlayNirvana.TicketModule.Integrations
{
    public interface IRoundModuleIntegration
    {
        IEnumerable<int> ActiveRoundIds();
    }

    public class RoundModuleIntegration : IRoundModuleIntegration
    {
        private readonly IRoundModuleExternal roundExternal;

        public RoundModuleIntegration(IRoundModuleExternal roundExternal)
        {
            this.roundExternal = roundExternal;
        }
        public IEnumerable<int> ActiveRoundIds()
        {
            return roundExternal.ActiveRoundIds();
        }
    }
}
