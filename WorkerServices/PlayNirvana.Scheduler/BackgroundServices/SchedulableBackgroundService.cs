using NCrontab;
using static NCrontab.CrontabSchedule;

namespace PlayNirvana.Scheduler.BackgroundServices
{
    public abstract class SchedulableBackgroundService : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        protected SchedulableBackgroundService()
        {
            var cronExpression = CronExpression();
            _schedule = Parse(cronExpression, new ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
        }

        public abstract string CronExpression();

        // Your job implementation
        public abstract Task JobAsync(CancellationToken ct);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                if (now >= _nextRun)
                {
                    try
                    {
                        await JobAsync(stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // normal shutdown
                    }
                    catch (Exception ex)
                    {
                        // TODO: inject ILogger and log the error
                        // _logger.LogError(ex, "Error running scheduled job.");
                    }

                    // Compute the *next* occurrence from "now"
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }

                // Yield the thread pool: sleep until the next run (or a short backoff)
                var delay = _nextRun - DateTime.UtcNow;
                if (delay < TimeSpan.FromMilliseconds(200))
                    delay = TimeSpan.FromMilliseconds(200); // protective floor

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutting down
                }
            }
        }
    }
}
