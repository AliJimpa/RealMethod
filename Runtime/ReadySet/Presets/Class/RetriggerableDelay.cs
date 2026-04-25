using System;
using System.Threading;
using System.Threading.Tasks;

public class RetriggerDelay
{
    private static int _nextId;
    public int Id { get; }

    private CancellationTokenSource _cts;
    private readonly float _delay;
    private bool _isRunning;

    public event Action<RetriggerDelay> OnCompleted;

    public RetriggerDelay(float delay)
    {
        Id = ++_nextId;
        _delay = delay;
    }

    public async void Trigger()
    {
        // Cancel any previous delay
        _cts?.Cancel();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _isRunning = true;

        try
        {
            // Convert seconds → milliseconds
            await Task.Delay((int)(_delay * 1000f), token);

            if (!token.IsCancellationRequested)
            {
                _isRunning = false;
                OnCompleted?.Invoke(this); // notifies external system to destroy/remove
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when retriggered
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
        _isRunning = false;
    }

    public bool IsRunning => _isRunning;
}
