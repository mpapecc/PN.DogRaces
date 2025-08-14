using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Services
{
    public class RoundService
    {
        private readonly RoundRepository roundRepository;
        private readonly IRepository<RaceDogResult> raceDogResultRepository;
        private readonly int betLockBeforeStart = 2;
        private readonly int raceDuration = 5;
        private readonly int newRoundsThreshold = 10;
        private readonly int roundDuration = 10;

        public RoundService(RoundRepository roundRepository, IRepository<RaceDogResult> raceDogResultRepository)
        {
            this.roundRepository = roundRepository;
            this.raceDogResultRepository = raceDogResultRepository;
        }

        public IList<Round> GenerateRounds(DateTime referentDateTime)
        {
            var rounds = Enumerable.Range(0, newRoundsThreshold).Select(x => new Round()
            {
                Start = referentDateTime.AddMinutes(x * roundDuration),
                RoundStatus = RoundStatus.Idle,
            }).ToList();

            return rounds;
        }

        public IList<Round> GenerateRounds(DateTime referentDateTime, Func<IList<Round>, IList<Round>> processFunc)
        {
            var rounds = GenerateRounds(referentDateTime);
            return processFunc(rounds);
        }

        public DateTime CalculateNextRoundStart()
        {
            var minutesToNextInterval = roundDuration - DateTime.UtcNow.Minute % roundDuration;
            return DateTime.UtcNow.AddMinutes(minutesToNextInterval);
        }

        public IList<Round> ActivateFirstNRounds(IList<Round> rounds, int roundsnNumber)
        {
            for (int i = 0; i < roundsnNumber; i++)
            {
                rounds[i].RoundStatus = RoundStatus.Active;
            }

            return rounds;
        }

        public void ActivateNextNRounds(int roundsNumber = 1)
        {
            roundRepository.GetNextNIdleRounds(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Active));
        }

        public IEnumerable<int> LockNextActiveRoundForBets(int roundsNumber = 1)
        {
            // maybe we can check when placing bet if there is less then 5 seconds before start 
            // and prevent bet...in that way we are reducing database updates but we have to make sure we 
            // are validating in code in every place!!!
            var nextRoundForActivationQuery = roundRepository.GetNextRoundForActivationQuery(roundsNumber);

            nextRoundForActivationQuery.ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Locked));

            return nextRoundForActivationQuery.Select(x => x.Id).ToList();
        }

        public void StartLockedRound(int roundsNumber = 1)
        {
            roundRepository.LockedRoundQuery()
               .OrderBy(x => x.Start)
               .Take(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.InProgress));
        }

        public void FinishInProgressRound(int roundsNumber = 1)
        {
            roundRepository.InProgressRoundQuery()
               .OrderBy(x => x.Start)
               .Take(roundsNumber)
               .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Finished));
        }

        public void GenerateRoundOutcome(IEnumerable<int> roundIds)
        {
            foreach (var roundId in roundIds)
            {
                var outcome = GenerateRandomLoop(GetRacingDogsList())
                .Select((x, i) => new RaceDogResult { RacingDogId = x.Id, Place = i + 1, RoundId = roundId }).ToList();

                this.raceDogResultRepository.InsertRange(outcome);
            }

            this.raceDogResultRepository.Commit();
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
