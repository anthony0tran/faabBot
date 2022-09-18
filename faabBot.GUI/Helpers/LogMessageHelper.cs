using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            _mainWindow.logTextBox.Text += string.Format("{0}:{1}\n", DateTime.Now.ToString("HH:mm:ss", ci), message);
        }
    }
}
