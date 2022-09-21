using faabBot.GUI.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace faabBot.GUI.Validators
{
    internal static class MainValidator
    {
        #region URL validators
        public static bool UrlValidator(TextBox textBox, Window window)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || !IsUrlValid(textBox.Text))
            {
                InputFieldHelper.SetErrorBorders(textBox, window);

                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("URL input is empty");
                }
                else if (!IsUrlValid(textBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("URL is invalid");
                }

                return false;
            }

            InputFieldHelper.ClearBorders(textBox, window);
            return true;
        }

        public static bool IsURLSet(string? url, TextBox textBox, Window window)
        {
            if (string.IsNullOrEmpty(url))
            {
                InputFieldHelper.SetErrorBorders(textBox, window);
                MsgWindowHelper.ShowErrorMsgWindow("No URL is set");

                return false;
            }

            return true;
        }

        private static bool IsUrlValid(string url)
        {
            var tryCreateResult = Uri.TryCreate(url, UriKind.Absolute, out _);
            return tryCreateResult;
        }
        #endregion URL validators

        public static bool ClientNameValidator(TextBox textBox, Window window)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || !IsValidFileName(textBox.Text))
            {
                InputFieldHelper.SetErrorBorders(textBox, window);

                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("Client input is empty");
                }

                if (!IsValidFileName(textBox.Text) && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MsgWindowHelper.ShowErrorMsgWindow("Client name contains invalid filename characters");
                }

                return false;
            }

            InputFieldHelper.ClearBorders(textBox, window);
            return true;
        }

        public static bool IsValidFileName(string name)
        {
            var invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            if (name.ToCharArray().Intersect(invalidFileNameChars).Any())
            {
                return false;
            }

            return true;
        }
    }
}
