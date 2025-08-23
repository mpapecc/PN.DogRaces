namespace PlayNirvana.RoundModule.Presentation.RoundHub
{
    public interface IRoundHubClient
    {
        Task RoundStarted(int roundId);
        Task RaceStartWithBetLock(int roundId);
        Task RoundFinished(object roundResult);
    }
}
