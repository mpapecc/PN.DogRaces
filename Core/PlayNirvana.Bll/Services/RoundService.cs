using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.Models;
using PlayNirvana.Bll.Repositories;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Services
{
    public class RoundService
    {
        private readonly IRoundRepository roundRepository;
        private readonly IRepository<RaceDogResult> raceDogResultRepository;
        private readonly int newRoundsThreshold = 30;
        private readonly int minimunActiveRounds = 5;


        public RoundService(IRoundRepository roundRepository, IRepository<RaceDogResult> raceDogResultRepository)
        {
            this.roundRepository = roundRepository;
            this.raceDogResultRepository = raceDogResultRepository;
        }

        public void TranslateActiveAndIdleRoundsStartInFuture()
        {
            this.roundRepository.Sp_TranslateActiveAndIdleRoundsStartInFuture();
        }

        public void GenerateRoundIfNeeded()
        {
            // betting time (7) + race (3) = 10 min
            // that means in a one day there can be 144 races
            // we are actually generating 216 races (days and half worth) so that we dont have
            // issues in case of latency in midnight
            // we will also check if ther are more then 200 iddle races in if so we will skipp generation

            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            var activeRoundsCount = roundRepository.GetActiveRoundsCount();

            if (idleRoundsCount >= this.newRoundsThreshold && activeRoundsCount > this.minimunActiveRounds)
            {
                return;
            }

            if (idleRoundsCount == 0)
            {

                GenerateRounds(processFunc: rounds => ActivateFirstNRounds(rounds, this.minimunActiveRounds + 5));
            }
            else if (idleRoundsCount > 0 && idleRoundsCount < this.newRoundsThreshold)
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStart();
                GenerateRounds(lastRoundStartTime);
            }
            else if (activeRoundsCount <= this.minimunActiveRounds + 2)
            {
                ActivateIdleRoundsAsync(this.minimunActiveRounds + 5);
            }

            return;
        }

        private void GenerateRounds(DateTime? referentDateTime = null, Func<IList<Round>, IList<Round>>? processFunc = null)
        {
            referentDateTime = referentDateTime ?? RoundTimeSlotGenerator.NextEvenMinuteUtc();

            var rounds = Enumerable.Range(0, newRoundsThreshold)
                .Select(x => new Round()
                {
                    Start = referentDateTime.Value.AddSeconds(x * RoundModel.roundDurationInSeconds),
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

        public RoundModel GetNextActiveRoundModel()
        {
            var nextRoundStartData = this.roundRepository.GetNextRoundForExecutionQuery()
                .Select(x => new RoundModel(x.Id, x.Start))
                .FirstOrDefault();

            if (nextRoundStartData?.Start == null)
            {
                //should handle this more gracefully, althoug this should never happen since RoundsGenerator makes sure there are always active rounds
                //maybe in this case manually call GenerateRounds()
                throw new InvalidOperationException("There are no active rounds for locking");
            }

            return nextRoundStartData;
        }

        public IEnumerable<RoundModel> GetActiveRounds()
        {
            return this.roundRepository.ActiveRoundQuery()
                .Select(x => new RoundModel(x.Id, x.Start))
                .ToList();
        }

        public void ActivateRound(int roundId)
        {
            this.roundRepository.IdleRoundQuery()
                                .Where(x => x.Id == roundId)
                                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
        }

        public Task ActivateIdleRoundsAsync(int roundsCount)
        {
            return this.roundRepository.IdleRoundQuery()
                                .OrderBy(x => x.Start)
                                .Take(roundsCount)
                                .ExecuteUpdateAsync(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
        }

        public Task LockRoundAsync(int roundId)
        {
            return this.roundRepository.Query()
                .Where(x => x.Id == roundId).
                ExecuteUpdateAsync(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Locked));

        }

        public Task FinishRoundAsync(int roundId)
        {
            return this.roundRepository.Query()
                .Where(x => x.Id == roundId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Finished));
        }

        public IEnumerable<RaceDogResultsRecord> GetRoundOutcome(int roundId)
        {
            return this.raceDogResultRepository.Query()
                .Where(x => x.RoundId == roundId)
                .OrderBy(x => x.Place)
                .Select(x => new RaceDogResultsRecord(x.RacingDogId, x.Place))
                .ToList();
        }

        public IEnumerable<RaceDogResultsRecord> GenerateRoundOutcome(int roundId)
        {
            var roundOutcome = GenerateRandomDogoList()
                .Select((x, i) => new RaceDogResult { RacingDogId = x.Id, Place = i + 1, RoundId = roundId }).ToList();

            this.raceDogResultRepository.InsertRange(roundOutcome);

            this.raceDogResultRepository.Commit();

            return roundOutcome.Select(x => new RaceDogResultsRecord(x.RacingDogId, x.RoundId));
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
