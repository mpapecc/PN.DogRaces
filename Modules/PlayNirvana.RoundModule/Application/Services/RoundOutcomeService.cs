using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Services
{
    public class RoundOutcomeService
    {
        private readonly IRoundModuleRepository<RaceDogResult> raceDogResultRepository;
        private readonly IRoundModuleRepository<RacingDog> racingDogRepository;

        public RoundOutcomeService(
            IRoundModuleRepository<RaceDogResult> raceDogResultRepository,
            IRoundModuleRepository<RacingDog> racingDogRepository)
        {
            this.raceDogResultRepository = raceDogResultRepository;
            this.racingDogRepository = racingDogRepository;
        }

        public IEnumerable<RaceDogResultDto> GenerateRoundOutcome(int roundId)
        {
            var listToShuffle = this.racingDogRepository.Query().ToList();

            var roundOutcome = ListRandomizer(listToShuffle)
                .Select((x, i) => new RaceDogResult { RacingDogId = x.Id, Place = i + 1, RoundId = roundId }).ToList();

            raceDogResultRepository.InsertRange(roundOutcome);

            raceDogResultRepository.Commit();

            return roundOutcome.Select(x => new RaceDogResultDto(x.RacingDogId, x.RoundId));
        }

        private IList<T> ListRandomizer<T>(IList<T> listToShuffle)
        {
            Random _rand = new Random();

            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }
    }
}
