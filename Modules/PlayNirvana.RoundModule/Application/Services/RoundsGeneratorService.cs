using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Enums;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Services
{
    public class RoundsGeneratorService
    {
        private readonly RoundOptions roundOptions;
        private readonly IRoundRepository roundRepository;

        public RoundsGeneratorService(
           IOptions<RoundOptions> roundOptions,
           IRoundRepository roundRepository)
        {
            this.roundOptions = roundOptions.Value;
            this.roundRepository = roundRepository;
        }

        public IEnumerable<Round> GenerateRoundIfNeeded()
        {
            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            var activeRoundsCount = roundRepository.GetActiveRoundsCount();
            var activeRounds = new List<Round>();

            if (idleRoundsCount >= this.roundOptions.NewRoundGenerationThreshold && activeRoundsCount > this.roundOptions.MinimunActiveRounds)
            {
                return activeRounds;
            }

            if (idleRoundsCount == 0)
            {
                activeRounds = GenerateRoundsAndReturnActive(activateRoundsCount : this.roundOptions.MinimunActiveRounds).ToList();
                return activeRounds;
            }

            if (idleRoundsCount > 0 && idleRoundsCount < this.roundOptions.NewRoundGenerationThreshold)
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStart();

                GenerateRoundsAndReturnActive(lastRoundStartTime);
            }
            else if (activeRoundsCount <= this.roundOptions.MinimunActiveRounds)
            {
                activeRounds.AddRange(ActivateIdleRounds(this.roundOptions.MinimunActiveRounds));
            }

            return activeRounds;
        }

        public void TranslateNonProcessedRoundsStartInFuture()
        {
            var referentDateTime = RoundTimeSlotGenerator.NextAlignedSlotUtc(this.roundOptions.RoundDurationInSeconds, DateTime.UtcNow);

            var activeAndIdleRounds = this.roundRepository.NonProcessedQuery().ToList();

            for (int i = 0; i < activeAndIdleRounds.Count; i++)
            {
                activeAndIdleRounds[i].Start = referentDateTime.AddSeconds(this.roundOptions.RoundDurationInSeconds * i);
            }

            this.roundRepository.Commit();
        }

        public bool IsFirstRoundForProcessStartInFuture()
        {
            var nextRoundStart = this.roundRepository.ActiveAnInProgressRoundQuery().Select(x => x.Start).FirstOrDefault();

            return nextRoundStart < DateTime.UtcNow;
        }

        private IEnumerable<Round> GenerateRoundsAndReturnActive(DateTime? referentDateTime = null, int activateRoundsCount = 0)
        {
            //we should check what are product requirements. time span of two minutes is selected by looking into existing solutions on web
            //also duration of 2 minutes or any other number which adds up to full hour (lets say 1, 1.5, 3, 4, 5, 6) whould be good choice
            //since it we would not have round start translation within every next hour and we could easily suport such configuration via appsettings or some env variable
            //for calcualtion next round start at the generation of rounds
            referentDateTime = referentDateTime ?? RoundTimeSlotGenerator.NextAlignedSlotUtc(this.roundOptions.RoundDurationInSeconds);

            var rounds = Enumerable.Range(0, this.roundOptions.NewRoundGenerationThreshold)
                .Select((x, i) => new Round()
                {
                    Start = referentDateTime.Value.AddSeconds(x * this.roundOptions.RoundDurationInSeconds),
                    RoundStatus = i < activateRoundsCount ? RoundStatus.Active : RoundStatus.Idle,
                }).ToList();

            roundRepository.InsertRange(rounds);
            roundRepository.Commit();

            return rounds.Where(x => x.RoundStatus == RoundStatus.Active);
        }

        private IList<Round> ActivateFirstNRounds(IList<Round> rounds, int roundsnNumber)
        {
            for (int i = 0; i < roundsnNumber; i++)
            {
                rounds[i].RoundStatus = RoundStatus.Active;
            }

            return rounds;
        }

        private IEnumerable<Round> ActivateIdleRounds(int roundsCount)
        {

            var rounds = roundRepository.IdleRoundQuery()
                                .OrderBy(x => x.Start)
                                .Take(roundsCount)
                                .ToList();

            rounds.ForEach(x => x.RoundStatus = RoundStatus.Active);

            this.roundRepository.Commit();

            return rounds;
        }

    }
}
