namespace PlayNirvana.RoundModule.Application.Models
{
    public class RoundModel
    {
        public static readonly int roundDurationInSeconds = 120;
        public static readonly int betLockBeforeStartInSeconds = 5;
        public static readonly int raceDurationInSeconds = 10;

        public int Id { get; init; }
        public DateTime Start { get; init; }
        public DateTime RaceStartWitBetLockDateTime { get; init; }

        public RoundModel(int id, DateTime start)
        {
            Id = id;
            Start = start;
            RaceStartWitBetLockDateTime = CalculateRoundRaceStartWithLock(start);
        }

        public TimeSpan CalculateUntilRoundFinish()
        {
            return Start.AddSeconds(roundDurationInSeconds) - DateTime.UtcNow;
        }

        public DateTime CalculateRoundFinishDate()
        {
            return Start.AddSeconds(roundDurationInSeconds);
        }

        public TimeSpan CalculateUntilRaceStartWitBetLock()
        {
            return RaceStartWitBetLockDateTime - DateTime.UtcNow;
        }

        public static DateTime CalculateRoundRaceStartWithLock(DateTime roundStart)
        {
            return roundStart.Add(TimeSpan.FromSeconds(DurationFromRoundStartToRaceStart()));
        }
        public static int DurationFromRoundStartToRaceStart()
        {
            return roundDurationInSeconds - (raceDurationInSeconds + betLockBeforeStartInSeconds);
        }
    };
}
