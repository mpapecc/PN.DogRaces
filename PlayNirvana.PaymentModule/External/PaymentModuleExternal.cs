using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.PaymentModule.Application;

namespace PlayNirvana.PaymentModule.External
{
    public class PaymentModuleExternal : IPaymentModuleExternal
    {
        private readonly PaymentService paymentService;

        public PaymentModuleExternal(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public void ProcessReservation(Guid reservationId, bool isWinningTicket)
        {
            this.paymentService.ProcessReservation(reservationId, isWinningTicket);
        }

        public void RemoveReservation(Guid reservationId)
        {
            this.paymentService.RemoveReservation(reservationId);
        }

        public Guid ReserveAmount(double amount)
        {
            return this.paymentService.ReserveAmount(amount);
        }
    }
}
