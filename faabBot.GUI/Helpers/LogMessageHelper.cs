using System;
using System.Globalization;

namespace faabBot.GUI.Helpers
{
    internal class LogMessageHelper
    {
        private MainWindow _mainWindow;

        public LogMessageHelper(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Log(string message)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            _mainWindow.logTextBox.Text += string.Format("{0}: {1}\n", DateTime.Now.ToString("HH:mm:ss", ci), message);
        }
    }
}
