using Microsoft.Extensions.Time.Testing;

namespace StopWatch.Tests;

public class StopWatchTests
{
    [Fact]
    public void RestartStopWatch()
    {
        var clock = new FakeTimeProvider(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
        var delay = TimeSpan.FromHours(2);
        var stopWatch = new BasicStopWatch(clock);

        stopWatch.Start();
        clock.Advance(delay);
        stopWatch.Stop();

        stopWatch.Start();
        clock.Advance(delay);
        stopWatch.Stop();

        Assert.NotNull(stopWatch.ElapsedTimeSpan);
        Assert.Equal(stopWatch.ElapsedTimeSpan, delay);
    }
}
