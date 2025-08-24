namespace PlayNirvana.CommonModule.Interfaces
{
    public interface IPaymentModuleExternal
    {
        Guid ReserveAmount(double amount);
        void RemoveReservation(Guid reservationId);
        void ProcessReservation(Guid reservationId, bool isWinningTicket);
    }
}
