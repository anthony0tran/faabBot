﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Url { get; set; }

        public int ProductId { get; set; }

        public override string ToString()
        {
            return Url!.Length > Globals.MaxUrlDisplayLength ? 
                string.Format("{0}: {1}", Id, Url![^Globals.MaxUrlDisplayLength..]) :
                string.Format("{0}: {1}", Id, Url);
        }
    }
}
