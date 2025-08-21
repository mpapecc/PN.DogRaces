using PlayNirvana.Common.Interfaces;
using PlayNirvana.RoundModule.Application.Repositories;

namespace PlayNirvana.RoundModule.External
{
    public class RoundModuleExternal : IRoundModuleExternal
    {
        private readonly IRoundRepository roundRepository;

        public RoundModuleExternal(IRoundRepository roundRepository)
        {
            this.roundRepository = roundRepository;
        }

        public IEnumerable<int> ActiveRoundIds()
        {
            return roundRepository.ActiveRoundQuery().Select(x => x.Id).ToList();
        }
    }
}
