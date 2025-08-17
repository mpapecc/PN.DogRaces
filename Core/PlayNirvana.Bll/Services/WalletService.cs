using PlayNirvana.Shared.Enums;
using PlayNirvana.Shared.Exceptions;

namespace PlayNirvana.Bll.Services
{
    public class WalletService
    {
        private double credits = 100;
        private IList<CreditReservation> creditReservations = new List<CreditReservation>();
        
        public void ReserveAmonunt(int ticketId, double amount)
        {
            if(amount <= 0)
                throw new WalletOperationException("Amount must be greater then 0");

            if (amount > this.credits)
                throw new WalletOperationException("Insufficent amount of credits");

            this.credits -= amount;
            creditReservations.Add(new CreditReservation(ticketId, amount));
        }

        public void RemoveReservation(double ticketId)
        {
            var reservation = this.creditReservations.FirstOrDefault(x => x.TicketId == ticketId);

            if (reservation == null)
                throw new WalletOperationException($"There is not reservation for ticket with id {ticketId}");

            this.credits += reservation.Amount;
            this.creditReservations.Remove(reservation);
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0)
                throw new WalletOperationException("Amount must be greater then 0");

            this.credits += amount;
        }

        public void ProcessReservation(double ticketId, TicketStatus ticketStatus)
        {
            var reservation = this.creditReservations.FirstOrDefault(x => x.TicketId == ticketId);

            if (reservation == null)
                throw new WalletOperationException($"There is not reservation for ticket with id {ticketId}");

            if (ticketStatus == TicketStatus.Won)
            {
                this.credits += reservation.Amount;
                this.creditReservations.Remove(reservation);
            }
            else if (ticketStatus == TicketStatus.Lost)
            {
                this.credits -= reservation.Amount;
                this.creditReservations.Remove(reservation);
            }
            else
                throw new WalletOperationException($"Cant process ticket with status {ticketStatus}");
        }
    }

    public record class CreditReservation(int TicketId, double Amount);
}
