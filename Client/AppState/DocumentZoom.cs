using DocsWASM.Shared;
using Microsoft.JSInterop;
using System.Timers;
using Timer = System.Timers.Timer;

namespace DocsWASM.Client.AppState
{
	public class DocumentZoom
	{
		public readonly double zoomMax = 1, zoomMin = 0.05;
		private static Func<double, Task> zoomChangeFunc;
		private Timer drawTimer;

		public double documentZoomLevel { get; private set; } = 1;
		public bool shouldRender = true;
		public bool mouseWheelZoom = false;
		public event Action RefreshZoomDocument;


		public void ChangeDocumentZoom(double val, bool mouseZoom = false)
		{
			mouseWheelZoom= mouseZoom;
			shouldRender = false;
			if (val <= zoomMax && val >= zoomMin)
				documentZoomLevel = val;
			if (val > zoomMax)
				documentZoomLevel = zoomMax;
			if (val < zoomMin)
				documentZoomLevel = zoomMin;
			RefreshZoomDocument.Invoke();
			drawTimer.Stop();
			drawTimer.Start();
		}

		[JSInvokable]
		public static async Task JSChangeDocumentZoom(double value)
		{
			await zoomChangeFunc.Invoke(value);
		}

		public DocumentZoom()
		{
			zoomChangeFunc = async(e) => ChangeDocumentZoom(e, true);
			drawTimer = new Timer(250);
			drawTimer.Elapsed += ((e, f) => { shouldRender = true; RefreshZoomDocument?.Invoke(); }) ;
			drawTimer.AutoReset = false;
		}
	}
}
