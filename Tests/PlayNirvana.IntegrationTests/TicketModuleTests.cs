using Microsoft.Extensions.Options;
using PlayNirvana.IntegrationTests.Infrastruture;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.TicketModule.Application.Models;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Services;
using PlayNirvana.TicketModule.Common.Enums;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.IntegrationTests
{
    public class TicketModuleTests : IntegrationTestFixture
    {
        public TicketModuleTests(IntegrationTestWepAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public void ValidateAndCreateTicket_ShouldCreateTicketAndAssociatedBets()
        {
            this.scopeRunner.Run<TicketService, ITicketModuleRepository<Bet>, IOptions<RoundOptions>>(
                (ticketService, betRepository, options) =>
                {
                    var roundOptions = options.Value;

                    var createTicketCommand = new CreateTicketCommand()
                    {
                        BetAmount = 5,
                        Bets = new List<BetModel>()
                        {
                            new BetModel()
                            {
                                BetType = BetType.Position,
                                RoundId = 1,
                                DogPositions = new List<DogPositionModel>()
                                {
                                    new DogPositionModel() { RacingDogId =1, Position = 2 }
                                }
                            }
                        }
                    };

                    var ticketId = ticketService.ValidateAndCreateTicket(createTicketCommand);

                    var ticketStatus = ticketService.CheckTicketStatus(ticketId);

                    Assert.True(ticketStatus == TicketStatus.Success,
                        $"Expected ticket to be in status {TicketStatus.Success} but its actually in {ticketStatus} status");

                    var bets = betRepository.Query().Where(x => x.TicketId == ticketId).ToList();

                    Assert.True(bets.Count == createTicketCommand.Bets.Count(),
                        "Expected bets count to be same as in CreateTicketCommand parameter");
                });
        }

    }
}
