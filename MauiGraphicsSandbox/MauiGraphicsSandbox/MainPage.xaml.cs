using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MauiGraphicsSandbox;

public partial class MainPage : ContentPage
{
	private readonly ILogger _logger;
	public MainPage(GraphicsDrawable renderer, ILogger<MainPage> logger)
	{
		_logger = logger;
		_renderer = renderer;
		InitializeComponent();
		BindingContext = this;

		timer.Elapsed += Timer_Elapsed;
		timer.Start();
	}

	System.Timers.Timer timer = new System.Timers.Timer(25);
	public GraphicsDrawable _renderer { get; set; }

	public string Framerate { get; set; }

	private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		try
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			graphicsView.Invalidate();

			stopwatch.Stop();
			Framerate = $"{stopwatch.ElapsedTicks}";
			_logger.LogInformation($"{stopwatch.ElapsedMilliseconds}");
			OnPropertyChanged("Framerate");
		}
		catch
		{

		}
	}
}

