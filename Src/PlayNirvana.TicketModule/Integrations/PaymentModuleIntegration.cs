using PlayNirvana.CommonModule.Interfaces;

namespace PlayNirvana.TicketModule.Integrations
{
    public interface IPaymentModuleIntegration
    {
        Guid ReserveAmount(double amount);
        void RemoveReservation(Guid reservationId);
        void ProcessReservation(Guid reservationId, bool isWinningTicket);
    }

    public class PaymentModuleIntegration : IPaymentModuleIntegration
    {
        private readonly IPaymentModuleExternal paymentModuleExternal;

        public PaymentModuleIntegration(IPaymentModuleExternal paymentModuleExternal)
        {
            this.paymentModuleExternal = paymentModuleExternal;
        }

        public void ProcessReservation(Guid reservationId, bool isWinningTicket)
        {
            this.paymentModuleExternal.ProcessReservation(reservationId, isWinningTicket);
        }

        public void RemoveReservation(Guid reservationId)
        {
            this.paymentModuleExternal.RemoveReservation(reservationId);
        }

        public Guid ReserveAmount(double amount)
        {
            return this.paymentModuleExternal.ReserveAmount(amount); 
        }
    }
}
