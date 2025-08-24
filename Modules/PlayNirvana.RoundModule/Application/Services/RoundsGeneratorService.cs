using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
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
            if (ShouldTranslateRoundInFuture())
            {
                TranslateActiveAndIdleRoundsStartInFuture();
            }

            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            var activeRoundsCount = roundRepository.GetActiveRoundsCount();

            if (idleRoundsCount >= this.roundOptions.NewRoundGenerationThreshold && activeRoundsCount > this.roundOptions.MinimunActiveRounds)
            {
                return Enumerable.Empty<Round>();
            }

            if (idleRoundsCount == 0)
            {
                return GenerateRounds(processFunc: rounds => ActivateFirstNRounds(rounds, this.roundOptions.MinimunActiveRounds));
            }
            else if (idleRoundsCount > 0 && idleRoundsCount < this.roundOptions.NewRoundGenerationThreshold)
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStart();

                GenerateRounds(lastRoundStartTime);

                return Enumerable.Empty<Round>();
            }
            else if (activeRoundsCount <= this.roundOptions.MinimunActiveRounds)
            {
                return ActivateIdleRounds(this.roundOptions.MinimunActiveRounds);
            }

            return Enumerable.Empty<Round>();
        }

        private void TranslateActiveAndIdleRoundsStartInFuture()
        {
            this.roundRepository.Sp_TranslateActiveAndIdleRoundsStartInFuture();
        }

        private bool ShouldTranslateRoundInFuture()
        {
            var nextActiveRoundStart = this.roundRepository.ActiveRoundQuery().Select(x => x.Start).FirstOrDefault();

            return nextActiveRoundStart < DateTime.UtcNow;
        }

        private IEnumerable<Round> GenerateRounds(DateTime? referentDateTime = null, Func<IList<Round>, IList<Round>>? processFunc = null)
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

            return rounds;
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
