namespace PlayNirvana.RoundModule.Application.Models
{
    public class RaceDogResultDto
    {
        public RaceDogResultDto(int racingDogId, int place)
        {
            RacingDogId = racingDogId;
            Place = place;
        }
        public int RacingDogId { get; set; }
        public int Place { get; set; }
    }
}
