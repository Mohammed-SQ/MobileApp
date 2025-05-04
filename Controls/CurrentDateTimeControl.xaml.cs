using System.Threading;

namespace FMMSRestaurant.Controls;

public partial class CurrentDateTimeControl : ContentView
{
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource _cancellationTokenSource;

    public CurrentDateTimeControl()
    {
        InitializeComponent();

        _cancellationTokenSource = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _ = UpdateTimeLabelEachSecond(_cancellationTokenSource.Token); // Start async task
    }

    private void UpdateTimeLabel()
    {
        if (this.Handler?.MauiContext == null) return; // Ensure context is available

        DateTime now = DateTime.Now;
        string formattedTime = $"{now:dddd, HH:mm:ss}";
        dayTimeLabel.Text = char.ToUpper(formattedTime[0]) + formattedTime.Substring(1);

        string formattedDate = $"{now:dd MMMM yyyy}";
        dateLabel.Text = formattedDate.Substring(0, 3) + char.ToUpper(formattedDate[3]) + formattedDate.Substring(4);
    }

    private async Task UpdateTimeLabelEachSecond(CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken))
        {
            UpdateTimeLabel();
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler == null && _cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}