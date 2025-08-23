using PlayNirvana.PaymentModule.Common;

namespace PlayNirvana.PaymentModule.Application
{
    public class PaymentService
    {
        public double credits { get; private set; } = 100;
        private IList<CreditReservation> creditReservations = new List<CreditReservation>();

        public Guid ReserveAmount(double amount)
        {
            if (amount <= 0)
                throw new WalletOperationException("Amount must be greater then 0");

            if (amount > this.credits)
                throw new WalletOperationException("Insufficent amount of credits");

            this.credits -= amount;
            var newReservation = new CreditReservation(amount);
            creditReservations.Add(newReservation);

            return newReservation.ReservationId;
        }

        public void RemoveReservation(Guid reservationId)
        {
            var reservation = this.creditReservations.FirstOrDefault(x => x.ReservationId == reservationId);

            if (reservation == null)
                throw new WalletOperationException($"There is not reservation for ticket with id {reservationId}");

            this.credits += reservation.Amount;
            this.creditReservations.Remove(reservation);
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0)
                throw new WalletOperationException("Amount must be greater then 0");

            this.credits += amount;
        }

        public void ProcessReservation(Guid reservationId, bool isWinningTicket)
        {
            var reservation = this.creditReservations.FirstOrDefault(x => x.ReservationId == reservationId);

            if (reservation == null)
                throw new WalletOperationException($"There is not reservation with id {reservationId}");

            if (isWinningTicket)
            {
                this.credits += reservation.Amount;
                this.creditReservations.Remove(reservation);
            }
            else
            {
                this.creditReservations.Remove(reservation);
            }
        }
    }

    public class CreditReservation
    {
        public readonly double Amount;
        public readonly Guid ReservationId;

        public CreditReservation(double amount)
        {
            Amount = amount;
            ReservationId = Guid.NewGuid();
        }
    };
}
