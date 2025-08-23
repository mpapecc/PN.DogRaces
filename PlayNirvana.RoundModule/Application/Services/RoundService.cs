using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Enums;
using PlayNirvana.RoundModule.Common.Exceptions;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Services
{
    public class RoundService
    {
        private readonly RoundOptions roundOptions;
        private readonly IRoundRepository roundRepository;
        private readonly IRoundModuleRepository<RaceDogResult> raceDogResultRepository;

        public RoundService(
            IOptions<RoundOptions> roundOptions,
            IRoundRepository roundRepository, 
            IRoundModuleRepository<RaceDogResult> raceDogResultRepository)
        {
            this.roundOptions = roundOptions.Value;
            this.roundRepository = roundRepository;
            this.raceDogResultRepository = raceDogResultRepository;
        }

        public void TranslateActiveAndIdleRoundsStartInFuture()
        {
            roundRepository.Sp_TranslateActiveAndIdleRoundsStartInFuture();
        }

        public void GenerateRoundIfNeeded()
        {
            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            var activeRoundsCount = roundRepository.GetActiveRoundsCount();

            if (idleRoundsCount >= this.roundOptions.NewRoundGenerationThreshold && activeRoundsCount > this.roundOptions.MinimunActiveRounds)
            {
                return;
            }

            if (idleRoundsCount == 0)
            {
                GenerateRounds(processFunc: rounds => ActivateFirstNRounds(rounds, this.roundOptions.MinimunActiveRounds + 5));
            }
            else if (idleRoundsCount > 0 && idleRoundsCount < this.roundOptions.NewRoundGenerationThreshold)
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStart();
                GenerateRounds(lastRoundStartTime);
            }
            else if (activeRoundsCount <= this.roundOptions.MinimunActiveRounds + 2)
            {
                ActivateIdleRoundsAsync(this.roundOptions.MinimunActiveRounds + 5);
            }

            return;
        }

        private void GenerateRounds(DateTime? referentDateTime = null, Func<IList<Round>, IList<Round>>? processFunc = null)
        {
            referentDateTime = referentDateTime ?? RoundTimeSlotGenerator.NextEvenMinuteUtc();

            var rounds = Enumerable.Range(0, this.roundOptions.NewRoundGenerationThreshold)
                .Select(x => new Round()
                {
                    Start = referentDateTime.Value.AddSeconds(x * RoundDto.roundDurationInSeconds),
                    RoundStatus = RoundStatus.Idle,
                }).ToList();

            if (processFunc != null)
                rounds = (List<Round>)processFunc(rounds);

            roundRepository.InsertRange(rounds);
            roundRepository.Commit();
        }

        private IList<Round> ActivateFirstNRounds(IList<Round> rounds, int roundsnNumber)
        {
            for (int i = 0; i < roundsnNumber; i++)
            {
                rounds[i].RoundStatus = RoundStatus.Active;
            }

            return rounds;
        }

        public RoundDto GetNextActiveRoundModel()
        {
            var nextRoundStartData = roundRepository.GetNextRoundForExecutionQuery()
                .Select(x => new RoundDto(x.Id, x.Start))
                .FirstOrDefault();

            if (nextRoundStartData?.Start == null)
            {
                //should handle this more gracefully, althoug this should never happen since RoundsGenerator makes sure there are always active rounds
                //maybe in this case manually call GenerateRounds()
                throw new NoActiveRoundsException("There are no active rounds for locking");
            }

            return nextRoundStartData;
        }

        public IEnumerable<RoundDto> GetActiveRounds()
        {
            return roundRepository.ActiveRoundQuery()
                .Select(x => new RoundDto(x.Id, x.Start))
                .ToList();
        }

        public void ActivateRound(int roundId)
        {
            roundRepository.IdleRoundQuery()
                                .Where(x => x.Id == roundId)
                                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
        }

        public Task ActivateIdleRoundsAsync(int roundsCount)
        {
            return roundRepository.IdleRoundQuery()
                                .OrderBy(x => x.Start)
                                .Take(roundsCount)
                                .ExecuteUpdateAsync(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
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

        public IEnumerable<RaceDogResultDto> GenerateRoundOutcome(int roundId)
        {
            var roundOutcome = GenerateRandomDogoList()
                .Select((x, i) => new RaceDogResult { RacingDogId = x.Id, Place = i + 1, RoundId = roundId }).ToList();

            raceDogResultRepository.InsertRange(roundOutcome);

            raceDogResultRepository.Commit();

            return roundOutcome.Select(x => new RaceDogResultDto(x.RacingDogId, x.RoundId));
        }

        //move this generation into some service
        private List<RacingDog> GenerateRandomDogoList()
        {
            Random _rand = new Random();

            var listToShuffle = new List<RacingDog>()
            {
                new RacingDog{Id = 1, Name = "Dogo1", Number  = 1},
                new RacingDog{Id = 2, Name = "Dogo2", Number  = 2},
                new RacingDog{Id = 3, Name = "Dogo3", Number  = 3},
                new RacingDog{Id = 4, Name = "Dogo4", Number  = 4},
                new RacingDog{Id = 5, Name = "Dogo5", Number  = 5},
                new RacingDog{Id = 6, Name = "Dogo6", Number  = 6},
                new RacingDog{Id = 7, Name = "Dogo7", Number  = 7},
                new RacingDog{Id = 8, Name = "Dogo8", Number  = 8},
                new RacingDog{Id = 9, Name = "Dogo9", Number  = 9},
            };

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
