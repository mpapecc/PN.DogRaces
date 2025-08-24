using PlayNirvana.RoundModule.Application.Models;

namespace PlayNirvana.RoundModule.Application
{
    public class ActiveRoundCache : Queue<RoundDto>
    {
        public IEnumerable<int> GetRoundIdList()
        {
            return this.Select(x => x.Id);
        }

        public void EnqueueList(IEnumerable<RoundDto> roundDtos)
        {
            foreach (var roundDto in roundDtos)
            {
                this.Enqueue(roundDto);
            }
        }
    }
}
