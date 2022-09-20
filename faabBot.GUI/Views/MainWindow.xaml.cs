﻿using faabBot.GUI.Controllers;
using faabBot.GUI.EventArguments;
using faabBot.GUI.Helpers;
using faabBot.GUI.Validators;
using faabBot.GUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace faabBot.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? URL;
        private SizeController SizesInstance { get; set; }
        private ProductController ProductInstance { get; set; }
        public LogController LogInstance { get; set; }

        private HashSet<string> ProductQueue { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            Title += string.Format(" v{0:f1}", Globals.Version);

            SizesInstance = new();
            ProductInstance = new();
            LogInstance = new(this);

            LogInstance.LogEventRaised += MainWindow_LogMessage;

            sizesListBox.ItemsSource = SizesInstance.Sizes;
            productsListBox.ItemsSource = ProductInstance.ProductQueue;
        }

        void MainWindow_LogMessage(object? sender, LogEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LogInstance.Log(e.Message!, e.Created);
            });
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new();
            aboutWindow.Show();
        }

        private void UrlOKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainValidator.UrlValidator(urlTextBox))
            {
                URL = urlTextBox.Text;

                urlTextBox.Clear();
                if (URL.Length > Globals.MaxUrlDisplayLength)
                {
                    urlStatsLbl.Content = string.Format("URL: {0}...", URL[..Globals.MaxUrlDisplayLength]);
                }
                else
                {
                    urlStatsLbl.Content = string.Format("URL: {0}", URL);
                }

                urlStatsLbl.ToolTip = URL;
            }
        }

        private void UrlClearBtn_Click(object sender, RoutedEventArgs e)
        {
            urlStatsLbl.Content = "URL:";
            urlTextBox.Text = "";
            URL = null;
            InputFieldHelper.ClearBorders(urlTextBox);
        }

        private void AddSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSizeWindow addSizeWindow = new(SizesInstance);
            addSizeWindow.ShowDialog();
        }

        private void DeleteSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sizesListBox.SelectedItem != null)
            {
                SizesInstance.Sizes.Remove(sizesListBox.SelectedItem.ToString()!);
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            LogInstance.CreateLogEvent("Session started, please wait...", DateTime.Now);

            Thread mainThread = new(x => StartSession());

            mainThread.Start();

            foreach (var productUrl in ProductQueue)
            {
                ProductInstance.ProductQueue.Add(productUrl);
            }
        }

        private void StartSession()
        {
            if (MainValidator.IsURLSet(URL, urlTextBox))
            {
                var instance = new SeleniumController(URL!, this);

                ProductQueue = instance.GetAllProductUrls();

                instance.CloseDriver();
            }
        }
    }
}
