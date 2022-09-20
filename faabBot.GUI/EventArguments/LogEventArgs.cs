using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.EventArguments
{
    public class LogEventArgs : EventArgs
    {
        public DateTime Created { get; set; }
        public string? Message { get; set; }
    }
}
