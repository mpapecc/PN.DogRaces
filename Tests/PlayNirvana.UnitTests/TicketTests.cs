using Microsoft.Extensions.Options;
using NSubstitute;
using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.Common.Options;
using PlayNirvana.TicketModule.Domain.Entites;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.UnitTests
{
    public class TicketTests
    {
        [Fact]
        public void ValidateTicketBetAmount_Should_Fail()
        {

            var ticketOptions = Options.Create(new TicketOptions() { MaxBetAmount = 10 });
            var betAmountValidator = new TicketBetAmountValidator(ticketOptions);

            var ticket = new Ticket()
            {
                BetAmount = 100
            };

            var validationResult = betAmountValidator.Validate(ticket);

            Assert.False(validationResult.IsSucess);
        }

        [Fact]
        public void ValidateTicketWinAmount_Should_Fail()
        {
            var ticketOptions = Options.Create(new TicketOptions() { MaxWinAmount = 10 });

            var betAmountValidator = new TicketWinAmountValidator(ticketOptions);

            var ticket = new Ticket()
            {
                WinAmount = 100
            };

            var validationResult = betAmountValidator.Validate(ticket);

            Assert.False(validationResult.IsSucess);
        }

        [Fact]
        public void ValidateTicketRound_Should_Fail()
        {
            var roundModuleIntegration = Substitute.For<IRoundModuleIntegration>();
            var activeRoundIds = new[] { 2 };

            roundModuleIntegration
                .ActiveRoundIds()
                .Returns(activeRoundIds);

            var ticketRoundsValidator = new TicketRoundsValidator(roundModuleIntegration);

            var ticket = new Ticket()
            {
                Bets = new Bet[] { new Bet {RoundId = 1 } }
            };

            var validationResult = ticketRoundsValidator.Validate(ticket);

            Assert.False(validationResult.IsSucess);
        }
    }
}
