namespace PlayNirvana.RoundModule.Application.Models
{
    public class RoundDto
    {
        public int Id { get; init; }
        public DateTime Start { get; init; }

        public RoundDto(int id, DateTime start)
        {
            Id = id;
            Start = start;
        }

        public TimeSpan CalculateUntilRoundStart()
        {
            return Start - DateTime.UtcNow;
        }

        public TimeSpan CalculateUntilRoundFinish(int roundDurationInSeconds)
        {
            return Start.AddSeconds(roundDurationInSeconds) - DateTime.UtcNow;
        }

        public TimeSpan CalculateUntilRaceStartWitBetLock(int durationFromRoundStartToRaceStart)
        {
            return Start.AddSeconds(durationFromRoundStartToRaceStart) - DateTime.UtcNow;
        }
    }
}
