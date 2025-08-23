using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using static NCrontab.CrontabSchedule;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public abstract class SchedulableBackgroundService : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private readonly ILogger<SchedulableBackgroundService> logger;
        private DateTime _nextRun;

        protected SchedulableBackgroundService(ILogger<SchedulableBackgroundService> logger)
        {
            var cronExpression = CronExpression();
            _schedule = Parse(cronExpression, new ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
            this.logger = logger;
        }

        public abstract string CronExpression();

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
                    catch (Exception ex)
                    {
                        //inject ILogger and log the error
                        this.logger.LogCritical(ex, "SchedulableBackgroundService error!");
                    }

                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }

                var delay = _nextRun - DateTime.UtcNow;

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
