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
        private readonly RoundsForProcessCache activeRoundCache;

        public RoundsGeneratorService(
           IOptions<RoundOptions> roundOptions,
           IRoundRepository roundRepository,
           RoundsForProcessCache activeRoundCache)
        {
            this.roundOptions = roundOptions.Value;
            this.roundRepository = roundRepository;
            this.activeRoundCache = activeRoundCache;
        }

        public IEnumerable<Round> GenerateRoundIfNeeded()
        {
            var activeRoundsCount = this.activeRoundCache.Count;
            var activeRounds = new List<Round>();

            var activeRoundsMinimumWithSafety = this.roundOptions.MinimunActiveRounds + this.roundOptions.CalculateMinimunActiveRoundsSafetyAddition();

            if (activeRoundsCount > activeRoundsMinimumWithSafety)
            {
                return activeRounds;
            }
            else
            {
                var referentDate = roundRepository.GetLastRoundStart();

                if(referentDate < DateTime.UtcNow)
                {
                    referentDate = DateTime.UtcNow; 
                }

                return GenerateRoundsAndReturnActive(activeRoundsMinimumWithSafety, referentDate);
            }
        }

        public void TranslateNonProcessedRoundsStartInFuture()
        {
            var referentDateTime = RoundTimeSlotGenerator.NextAlignedSlotUtc(this.roundOptions.RoundDurationInSeconds, DateTime.UtcNow);

            var activeAndIdleRounds = this.roundRepository.RoundsForProcessQuery().ToList();

            for (int i = 0; i < activeAndIdleRounds.Count; i++)
            {
                activeAndIdleRounds[i].Start = referentDateTime.AddSeconds(this.roundOptions.RoundDurationInSeconds * i);
            }

            this.roundRepository.Commit();
        }

        public bool IsFirstRoundForProcessStartInPast()
        {
            var nextRoundStart = this.roundRepository.RoundsForProcessQuery().Select(x => x.Start).FirstOrDefault();

            return nextRoundStart < DateTime.UtcNow;
        }

        private IEnumerable<Round> GenerateRoundsAndReturnActive(int activeRoundsMinimumWithSafety, DateTime? referentDateTime = null)
        {
            //we should check what are product requirements.based on existing solution on web two minutes is good choice for round duration
            //also duration of 2 minutes or any other number which adds up to full hour (lets say 1, 1.5, 3, 4, 5, 6) whould be good choice
            //since it we would not have round start translation within every next hour and we could easily suport such configuration via appsettings or some env variable
            //for calcualtion next round start at the generation of rounds
            referentDateTime = RoundTimeSlotGenerator.NextAlignedSlotUtc(this.roundOptions.RoundDurationInSeconds);

            var rounds = Enumerable.Range(0, activeRoundsMinimumWithSafety)
                .Select((x, i) => new Round()
                {
                    Start = referentDateTime.Value.AddSeconds(x * this.roundOptions.RoundDurationInSeconds),
                    RoundStatus = RoundStatus.Active
                }).ToList();

            roundRepository.InsertRange(rounds);
            roundRepository.Commit();

            return rounds;
        }
    }
}
