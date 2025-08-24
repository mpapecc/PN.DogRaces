using PlayNirvana.PaymentModule.Application;
using PlayNirvana.PaymentModule.Common;

namespace PlayNirvana.UnitTests
{
    public class PaymentTests
    {

        [Fact]
        public void ReserveAmonunt_Should_ThrowWalletOperationExceptionIfAmountIsBiggerThenCredit()
        {
            var paymentService = new PaymentService();

            var reservationId = paymentService.ReserveAmount(1500);

            Assert.Throws<WalletOperationException>(() => paymentService.ReserveAmount(150));
        }

        [Fact]
        public void ReserveAmonunt_Should_Pass()
        {
            var paymentService = new PaymentService();
            var reservationAmount = 50;
            var reservationId = paymentService.ReserveAmount(reservationAmount);

            var expectedCreditsAfterReservation = 50;

            Assert.Equal(expectedCreditsAfterReservation, paymentService.credits);
        }

        [Fact]
        public void ProcessWiningReservation_Should_ThrowWalletOperationExceptionIfReservationIdNotValid()
        {
            var paymentService = new PaymentService();

            paymentService.ProcessReservation(Guid.NewGuid(), true);

            Assert.Throws<WalletOperationException>(() => paymentService.ProcessReservation(Guid.NewGuid(), true));
        }

        [Fact]
        public void ProcessWiningReservation_Should_Pass_AndAddReservationAmountToCredits()
        {
            var paymentService = new PaymentService();
            var reservationAmount = 50;
            var reservationId = paymentService.ReserveAmount(reservationAmount);

            paymentService.ProcessReservation(reservationId, true);

            var expectedCreditsAfterReservationProcess = 100;

            Assert.Equal(expectedCreditsAfterReservationProcess, paymentService.credits);
        }

        [Fact]
        public void ProcessWiningReservation_Should_PassAnd()
        {
            var paymentService = new PaymentService();
            var reservationAmount = 50;
            var reservationId = paymentService.ReserveAmount(reservationAmount);

            paymentService.ProcessReservation(reservationId, false);

            var expectedCreditsAfterReservationProcess = 50;

            Assert.Equal(expectedCreditsAfterReservationProcess, paymentService.credits);
        }
    }
}