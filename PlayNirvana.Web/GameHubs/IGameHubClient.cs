namespace PlayNirvana.Web.GameHubs
{
    public interface IGameHubClient
    {
        Task SendRoundResult(object roundResult);
    }
}
