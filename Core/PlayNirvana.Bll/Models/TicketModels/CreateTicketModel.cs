using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Models.TicketModels
{
    public class CreateTicketModel
    {
        public int TicketId { get; set; }
        public double BetAmount { get; set; } 
        public IEnumerable<BetModel> Bets { get; set; }

        public Ticket ToTicket()
        {
            return new Ticket() 
            {
                BetAmount = this.BetAmount,
                Bets = this.Bets.Select(b => new Bet() 
                { 
                    RoundId = b.RoundId, 
                    BetType = b.BetType,
                    DogPositions = b.DogPositions.Select(dp => new DogPosition() 
                    { 
                        RacingDogId = dp.RacingDogId,
                        Position = dp.Position,
                    }).ToList()
                }).ToList()
            };
        }
    }

    public class BetModel
    {
        public int RoundId { get; set; }
        public BetType BetType { get; set; }
        public IEnumerable<DogPositionModel> DogPositions { get; set; }
    }

    public class DogPositionModel
    {
        public int RacingDogId { get; set; }
        public int Position { get; set; }
    }
}
