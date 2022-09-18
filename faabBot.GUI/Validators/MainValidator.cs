﻿using faabBot.GUI.Helpers;
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
        #region URL validators
        public static bool UrlValidator(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || !IsUrlValid(textBox.Text))
            {
                InputFieldHelper.SetErrorBorders(textBox);

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

            InputFieldHelper.ClearBorders(textBox);
            return true;
        }

        public static bool IsURLSet(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
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
    }
}