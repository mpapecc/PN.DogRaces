namespace PlayNirvana.RoundModule.Common.Options
{
    public class RoundOptions
    {
        public int MinimunActiveRounds { get; set; }
        public int NewRoundGenerationThreshold { get; set; }
        public int RoundsGeneratorIntervalInMinutes { get; set; }
        public int RoundDurationInSeconds { get; set; }
        public int RoundLockBeforeRaceStart { get; set; }
        public int RaceDurationInSeconds { get; set; }
        public int MinimumRoundDurationBeforeLockInSeconds { get; set; }

        public int DurationFromRoundStartToRaceStart()
        {
            return RoundDurationInSeconds - (RaceDurationInSeconds + RoundLockBeforeRaceStart);
        }

        public int CalculateMinimunActiveRoundsSafetyAddition()
        {
            int generationSec = (int)TimeSpan.FromMinutes(RoundsGeneratorIntervalInMinutes).TotalSeconds;
            var roundsCountProcessedBetweenRoundsGenerations = (generationSec + RoundDurationInSeconds - 1) / RoundDurationInSeconds;;

            return roundsCountProcessedBetweenRoundsGenerations;
        }
    }
}
