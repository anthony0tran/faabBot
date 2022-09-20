using faabBot.GUI.EventArguments;
using faabBot.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.Controllers
{
    public class LogController
    {
        private readonly MainWindow _mainWindow;
        public event EventHandler<LogEventArgs>? NewLogCreated;

        public LogController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Log(string message, DateTime created)
        {
            LogMessageHelper.Log(message, created, _mainWindow);
        }

        public void NewLogCreatedEvent(string message, DateTime created)
        {
            LogEventArgs args = new()
            {
                Message = message,
                Created = created
            };

            OnNewLogCreatedEvent(args);
        }

        protected virtual void OnNewLogCreatedEvent(LogEventArgs @event)
        {
            NewLogCreated?.Invoke(this, @event);
        }
    }
}
