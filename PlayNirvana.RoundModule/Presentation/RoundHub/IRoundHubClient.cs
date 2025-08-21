namespace PlayNirvana.RoundModule.Presentation.RoundHub
{
    public interface IRoundHubClient
    {
        Task RoundStarted();
        Task RaceStartWithBetLock(int roundId);
        Task RoundFinished(object roundResult);
    }
}
