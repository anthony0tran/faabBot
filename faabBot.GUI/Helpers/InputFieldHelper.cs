using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace faabBot.GUI.Helpers
{
    internal class InputFieldHelper
    {
        public static void SetErrorBorders(TextBox textBox)
        {
            textBox.BorderBrush = Brushes.Red;
        }

        public static void ClearBorders(TextBox textBox)
        {
            textBox.ClearValue(Border.BorderBrushProperty);
        }
    }
}
