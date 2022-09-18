using faabBot.GUI.EventArguments;
using System;
using System.Globalization;

namespace faabBot.GUI.Helpers
{
    public class LogMessageHelper
    {
        private MainWindow _mainWindow;
        public event EventHandler<LogEventArgs> LogEventRaised;

        public LogMessageHelper(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Log(string message)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            _mainWindow.logTextBox.Text += string.Format("{0}: {1}\n", DateTime.Now.ToString("HH:mm:ss", ci), message);
        }

        public void CreateEvent(string message)
        {
            LogEventArgs args = new()
            {
                Message = message
            };

            OnLogEventRaised(args);
        }

        protected virtual void OnLogEventRaised(LogEventArgs e)
        {
            EventHandler<LogEventArgs> handler = LogEventRaised;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
