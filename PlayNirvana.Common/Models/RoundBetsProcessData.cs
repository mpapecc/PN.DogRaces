namespace PlayNirvana.Common.Models
{
    public class RoundBetsProcessData
    {
        public int RoundId { get; set; }
        public IEnumerable<RaceDogResultModel> RaceDogsResult { get; set; }
    }
}
