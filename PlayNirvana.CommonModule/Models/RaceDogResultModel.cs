namespace PlayNirvana.CommonModule.Models
{
    public class RaceDogResultModel
    {
        public RaceDogResultModel(int racingDogId, int place)
        {
            RacingDogId = racingDogId;
            Place = place;
        }
        public int RacingDogId { get; set; }
        public int Place { get; set; }
    }
}
