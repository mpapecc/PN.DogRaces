using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Services
{
    public class RoundService
    {
        private readonly RoundRepository roundRepository;
        private readonly IRepository<RaceDogResult> raceDogResultRepository;
        private readonly int newRoundsThreshold = 10;
        private readonly int roundDuration = 10;
        private readonly int minimunActiveRounds = 5;

        public RoundService(RoundRepository roundRepository, IRepository<RaceDogResult> raceDogResultRepository)
        {
            this.roundRepository = roundRepository;
            this.raceDogResultRepository = raceDogResultRepository;
        }

        public Task GenerateRounds()
        {
            // betting time (7) + race (3) = 10 min
            // that means in a one day there can be 144 races
            // we are actually generating 216 races (days and half worth) so that we dont have
            // issues in case of latency in midnight
            // we will also check if ther are more then 200 iddle races in if so we will skipp generation

            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            IEnumerable<Round> rounds;

            if (idleRoundsCount >= this.newRoundsThreshold)
            {
                return Task.CompletedTask;
            }

            // should add BeforeJob method to create rounds when Worker starts so that user
            // doesnt have to wait for next execution for placing bet
            if (idleRoundsCount == 0)
            {
                var nextRoundStartTime = CalculateNextRoundStart();

                rounds = GenerateRounds(nextRoundStartTime, rounds => ActivateFirstNRounds(rounds, this.minimunActiveRounds));
            }
            else
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStartDate();
                rounds = GenerateRounds(lastRoundStartTime);
            }

            roundRepository.InsertRange(rounds);
            roundRepository.Commit();

            return Task.CompletedTask;
        }

        private IList<Round> GenerateRounds(DateTime referentDateTime)
        {
            var rounds = Enumerable.Range(0, newRoundsThreshold).Select(x => new Round()
            {
                Start = referentDateTime.AddMinutes(x * roundDuration),
                RoundStatus = RoundStatus.Idle,
            }).ToList();

            return rounds;
        }

        private IList<Round> GenerateRounds(DateTime referentDateTime, Func<IList<Round>, IList<Round>> processFunc)
        {
            var rounds = GenerateRounds(referentDateTime);
            return processFunc(rounds);
        }

        private DateTime CalculateNextRoundStart()
        {
            var minutesToNextInterval = roundDuration - DateTime.UtcNow.Minute % roundDuration;
            return DateTime.UtcNow.AddMinutes(minutesToNextInterval);
        }

        private IList<Round> ActivateFirstNRounds(IList<Round> rounds, int roundsnNumber)
        {
            for (int i = 0; i < roundsnNumber; i++)
            {
                rounds[i].RoundStatus = RoundStatus.Active;
            }

            return rounds;
        }

        public void ActivateNextNRounds(int roundsNumber = 1)
        {
            this.roundRepository.GetNextNIdleRounds(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
        }

        public IEnumerable<int> LockNextActiveRoundForBets(int roundsNumber = 1)
        {
            // maybe we can check when placing bet if there is less then 5 seconds before start 
            // and prevent bet...in that way we are reducing database updates but we have to make sure we 
            // are validating in code in every place!!!
            var nextRoundForActivationQuery = this.roundRepository.GetNextRoundForActivationQuery(roundsNumber);

            if (!nextRoundForActivationQuery.Any())
            {
                return Enumerable.Empty<int>();
            }

            nextRoundForActivationQuery.ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Locked));

            return nextRoundForActivationQuery.Select(x => x.Id).ToList();
        }

        public void StartLockedRound(int roundsNumber = 1)
        {
            this.roundRepository.LockedRoundQuery()
               .OrderBy(x => x.Start)
               .Take(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.InProgress));
        }

        public void FinishInProgressRound(int roundsNumber = 1)
        {
            this.roundRepository.InProgressRoundQuery()
               .OrderBy(x => x.Start)
               .Take(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Finished));
        }

        public IEnumerable<RoundOutcome> GenerateRoundOutcome(IEnumerable<int> roundIds)
        {
            var outcomes = Enumerable.Empty<RoundOutcome>();
            
            foreach (var roundId in roundIds)
            {
                var roundOutcome = GenerateRandomLoop(GetRacingDogsList())
                .Select((x, i) => new RaceDogResult { RacingDogId = x.Id, Place = i + 1, RoundId = roundId }).ToList();

                this.raceDogResultRepository.InsertRange(roundOutcome);

                outcomes.Append(new RoundOutcome(roundId, roundOutcome.Select(x => new RaceDogResultsRecord(x.RacingDogId, x.RoundId))));
            }

            this.raceDogResultRepository.Commit();
            return outcomes;
        }

        //move this generation into some service
        private List<RacingDog> GetRacingDogsList()
        {
            return new()
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
        }

        private List<RacingDog> GenerateRandomLoop(List<RacingDog> listToShuffle)
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
