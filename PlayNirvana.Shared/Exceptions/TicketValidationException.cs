namespace PlayNirvana.Shared.Exceptions
{
    public class TicketValidationException : AggregateException
    {
        public TicketValidationException(string message): base(message)
        {
        }

        public TicketValidationException(IEnumerable<TicketValidationException> ticketValidationExceptions) : base(ticketValidationExceptions)
        {
        }
    }
}
