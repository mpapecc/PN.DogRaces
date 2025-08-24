using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.RoundModule.Application;
using PlayNirvana.RoundModule.Application.Repositories;

namespace PlayNirvana.RoundModule.External
{
    public class RoundModuleExternal : IRoundModuleExternal
    {
        private readonly ActiveRoundCache activeRoundCache;

        public RoundModuleExternal(ActiveRoundCache activeRoundCache)
        {
            this.activeRoundCache = activeRoundCache;
        }

        public IEnumerable<int> ActiveRoundIds()
        {
            return activeRoundCache.GetRoundIdList();
        }
    }
}
