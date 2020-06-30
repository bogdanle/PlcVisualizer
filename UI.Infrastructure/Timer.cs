using System;
using System.Windows.Threading;

namespace UI.Infrastructure
{
    public class Timer : DispatcherTimer
    {
        public Timer(int interval, bool oneShot, bool autoStart = false)
        {
            Interval = TimeSpan.FromMilliseconds(interval);
            OneShot = oneShot;
            Tick += Timer_Tick;

            if (autoStart)
            {
                Start();
            }
        }

        public bool OneShot { get; set; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (OneShot)
            {
                Stop();
            }
        }        
    }
}
