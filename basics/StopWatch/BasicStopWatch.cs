namespace StopWatch;

public class BasicStopWatch
{
    private readonly TimeProvider _timeProvider;
    private DateTimeOffset? _startTime;
    private DateTimeOffset? _stopTime;

    public BasicStopWatch(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public void Start()
    {
        _startTime = _timeProvider.GetUtcNow();
        _stopTime = null;
    }

    public void Stop()
    {
        if (_startTime != null)
            _stopTime = _timeProvider.GetUtcNow();
    }

    public TimeSpan? ElapsedTimeSpan
    {
        get
        {
            if (_startTime != null && _stopTime != null)
                return _stopTime - _startTime; // operator overloading operator -(datetime d1, datetime d2) => new TimeSpan(d1.Ticks - d1.Ticks)
            return null;
        }
    }
}