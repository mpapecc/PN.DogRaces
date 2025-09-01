using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.RoundModule.Application;

namespace PlayNirvana.RoundModule.External
{
    public class RoundModuleExternal : IRoundModuleExternal
    {
        private readonly RoundsForProcessCache activeRoundCache;

        public RoundModuleExternal(RoundsForProcessCache activeRoundCache)
        {
            this.activeRoundCache = activeRoundCache;
        }

        public IEnumerable<int> ActiveRoundIds()
        {
            return activeRoundCache.GetRoundIdList();
        }
    }
}
