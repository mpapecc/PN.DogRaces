using Microsoft.EntityFrameworkCore;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Enums;

namespace PlayNirvana.RoundModule.Application.Services
{
    public class RoundService
    {
        private readonly IRoundRepository roundRepository;

        public RoundService(
            IRoundRepository roundRepository)
        {
            this.roundRepository = roundRepository;
        }

        public void LockRound(int roundId)
        {
            roundRepository.Query()
                .Where(x => x.Id == roundId).
                ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Locked));

        }

        public void FinishRound(int roundId)
        {
            roundRepository.Query()
                .Where(x => x.Id == roundId)
                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Finished));
        }

        public IEnumerable<RoundDto> GetActiveRoundDtos()
        {
            return roundRepository.ActiveRoundQuery().Select(x => new RoundDto(x.Id, x.Start)).ToList();
        }
    }
}
