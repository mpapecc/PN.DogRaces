using Microsoft.AspNetCore.SignalR;

namespace PlayNirvana.Web.GameHubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        public Task SendRoundResult(object roundResult)
        {
            return Clients.All.SendRoundResult(roundResult);
        }
    }
}
