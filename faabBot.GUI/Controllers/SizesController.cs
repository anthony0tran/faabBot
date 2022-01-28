using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.Controllers
{
    public class SizesController
    {
        public ObservableCollection<string> Sizes { get; set; } = new();
    }
}
