namespace PlayNirvana.RoundModule.Application.Services
{
    public static class RoundTimeSlotGenerator
    {
        public static DateTime NextAlignedSlotUtc(int slotSeconds, DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;
            if (now.Kind != DateTimeKind.Utc)
                now = DateTime.SpecifyKind(now, DateTimeKind.Utc);

            var hourStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);
            var elapsedSeconds = (int)Math.Floor((now - hourStart).TotalSeconds);

            var nextIndex = (elapsedSeconds / slotSeconds) + 1;
            return hourStart.AddSeconds(nextIndex * slotSeconds);
        }
    }
}
