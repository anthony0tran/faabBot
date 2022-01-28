using faabBot.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace faabBot.GUI.Validators
{
    internal static class MainValidator
    {
        #region validators
        public static bool UrlValidator(MainWindow mainWindow)
        {
            if (String.IsNullOrWhiteSpace(mainWindow.urlTextBox.Text) || !IsUrlValid(mainWindow.urlTextBox.Text))
            {
                InputFieldHelper.SetErrorBorders(mainWindow.urlTextBox);

                if (String.IsNullOrWhiteSpace(mainWindow.urlTextBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("URL input is empty");
                } 
                else if (!IsUrlValid(mainWindow.urlTextBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("URL is invalid");
                }

                return false;
            }

            InputFieldHelper.ClearBorders(mainWindow.urlTextBox);
            return true;
        }

        private static bool IsUrlValid(string url)
        {
            var tryCreateResult = Uri.TryCreate(url, UriKind.Absolute, out _);
            return tryCreateResult;
        }
        #endregion validators
    }
}
