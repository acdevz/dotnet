using StopWatch;

var stopWatch = new BasicStopWatch();

stopWatch.Start();
for (int i = 0; i < 5; i++)
{
    await Task.Delay(1000);
}
stopWatch.Stop();

Console.WriteLine(stopWatch.ElapsedTimeSpan);