using faabBot.GUI.Helpers;
using faabBot.GUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faabBot.GUI.Validators
{
    internal static class AddSizeValidator
    {
        public static bool isNotAdded(string size, ObservableCollection<string> sizes)
        {
            if (sizes.Contains(size))
            {
                MsgWindowHelper.ShowErrorMsgWindow("Size already added");
                return false;
            }

            return true;
        }

        public static bool ValidateInputFields(AddSizeWindow addSizeWindow)
        {
            if (String.IsNullOrWhiteSpace(addSizeWindow.AddSizeTextBox.Text) && addSizeWindow.AddSizeComboBox.SelectedIndex == default)
            {
                MsgWindowHelper.ShowErrorMsgWindow("No size selected");
                return false;
            }

            return true;
        }
    }
}
