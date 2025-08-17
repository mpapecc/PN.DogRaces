namespace PlayNirvana.Web.GameHubs
{
    public interface IGameHubClient
    {
        Task RoundFinished(object roundResult);
        Task RoundStarted(object roundResult);
    }
}
