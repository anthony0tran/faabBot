using faabBot.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.EventArguments
{
    public class ProductEventArgs : EventArgs
    {
        public Product? Product { get; set; }
    }
}