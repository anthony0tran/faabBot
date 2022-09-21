using faabBot.GUI.Controllers;
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
        public SizeController SizesInstance { get; set; }
        public ProductController ProductInstance { get; set; }
        public LogController LogInstance { get; set; }

        public string? ClientName { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Title += string.Format(" v{0:f1}", Globals.Version);

            SizesInstance = new();
            LogInstance = new(this);
            ProductInstance = new(this);


            LogInstance.NewLogCreated += MainWindow_LogMessage;

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
            if (MainValidator.UrlValidator(urlTextBox, this))
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
            InputFieldHelper.ClearBorders(urlTextBox, this);
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
            Thread seleniumThread = new(x => StartSession());

            seleniumThread.Start();
        }

        private void StartSession()
        {
            if (MainValidator.IsURLSet(URL, urlTextBox, this))
            {
                var seleniumInstance = new SeleniumController(URL!, this);

                //Start scraping here
                var subImageDirectory = DirectoryHelper.CreateSubImageDirectory(this, LogInstance);
                seleniumInstance.DownloadVariations(subImageDirectory);

                LogInstance.NewLogCreatedEvent(string.Format("found {0} products", ProductInstance.ProductQueue.Count), DateTime.Now);

                seleniumInstance.CloseDriver();
            }
        }

        private void ClientNameClearBtn_Click(object sender, RoutedEventArgs e)
        {
            clientNameLabel.Content = "Client:";
            clientNameTxtBox.Text = "";
            ClientName = null;
            InputFieldHelper.ClearBorders(clientNameTxtBox, this);
        }

        private void ClientNameOkBtn_Click(object sender, RoutedEventArgs e)
        {            
            if (MainValidator.ClientNameValidator(clientNameTxtBox, this))
            {
                ClientName = clientNameTxtBox.Text;
                clientNameLabel.Content += ClientName;

                clientNameTxtBox.Clear();
            }
        }
    }
}
