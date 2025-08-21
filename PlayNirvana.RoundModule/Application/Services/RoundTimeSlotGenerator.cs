namespace PlayNirvana.RoundModule.Application.Services
{
    public static class RoundTimeSlotGenerator
    {
        public static DateTime NextEvenMinuteUtc()
        {
            return NextEvenMinuteUtc(DateTime.UtcNow);
        }

        public static DateTime NextEvenMinuteUtc(DateTime utcNow)
        {
            if (utcNow.Kind != DateTimeKind.Utc)
                utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc); // assumes input is already UTC

            // round up to the next whole minute, then bump to next even if needed
            var next = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0, DateTimeKind.Utc)
                       .AddMinutes(1);

            if ((next.Minute & 1) == 1)
                next = next.AddMinutes(1);

            next.AddSeconds(-next.Second);
            return next;
        }
    }
}
