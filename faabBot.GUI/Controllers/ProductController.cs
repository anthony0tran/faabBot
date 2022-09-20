using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.Controllers
{
    public class ProductController
    {
        public ObservableHashSet<string> ProductQueue { get; set; } = new();
    }
}
