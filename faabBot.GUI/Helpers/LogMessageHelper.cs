using faabBot.GUI.EventArguments;
using System;
using System.Globalization;
using System.Windows;

namespace faabBot.GUI.Helpers
{
    internal class LogMessageHelper
    {
        public static void Log(string message, DateTime created, MainWindow mainWindow)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            mainWindow.logTextBox.Text += string.Format("{0}: {1}\n", created.ToString("HH:mm:ss", ci), message);
        }
    }
}
