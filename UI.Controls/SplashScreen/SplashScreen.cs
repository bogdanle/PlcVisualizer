using System;
using System.Windows;
using System.Windows.Threading;

namespace UI.Controls
{
    public class SplashScreen : Window
    {
        private static SplashScreen _instance;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private SplashScreen()
        {
            _timer.Interval = TimeSpan.FromSeconds(4);
            _timer.Tick += OnTimerTick;
            _timer.Start();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.WidthAndHeight;
            ShowInTaskbar = false;

            Style = (Style)Application.Current.Resources["MySplashScreenStyle"];
        }

        public static SplashScreen Instance => _instance ?? (_instance = new SplashScreen());

        public bool IsClosed { get; private set; }

        public new void Hide()
        {
            VisualStateManager.GoToState(this, "Hide", false);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (!IsClosed)
            {
                Close();
                IsClosed = true;
            }
        }
    }
}
