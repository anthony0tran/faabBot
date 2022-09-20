using faabBot.GUI.EventArguments;
using System;
using System.Globalization;

namespace faabBot.GUI.Helpers
{
    public class LogMessageHelper
    {
        private readonly MainWindow _mainWindow;
        public event EventHandler<LogEventArgs>? LogEventRaised;

        public LogMessageHelper(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Log(string message, DateTime created)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            _mainWindow.logTextBox.Text += string.Format("{0}: {1}\n", created.ToString("HH:mm:ss", ci), message);
        }

        public void CreateLogEvent(string message, DateTime created)
        {
            LogEventArgs args = new()
            {
                Message = message,
                Created = created
            };

            OnLogEventRaised(args);
        }

        protected virtual void OnLogEventRaised(LogEventArgs e)
        {
            LogEventRaised?.Invoke(this, e);
        }
    }
}
