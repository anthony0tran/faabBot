using faabBot.GUI.Controllers;
using faabBot.GUI.Helpers;
using faabBot.GUI.Validators;
using faabBot.GUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private SizesController SizesInstance { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Title += String.Format(" v{0:f1}", Globals.Version);

            SizesInstance = new();

            sizesListBox.ItemsSource = SizesInstance.Sizes;
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new();
            aboutWindow.Show();
        }

        private void UrlOKBtn_Click(object sender, RoutedEventArgs e)
        {
            string urlText = urlTextBox.Text;
            if (MainValidator.UrlValidator(this))
            {
                urlTextBox.Clear();
                if (urlText.Length > Globals.MaxUrlDisplayLength)
                {
                    urlStatsLbl.Content = String.Format("URL: {0}...", urlText[..Globals.MaxUrlDisplayLength]);
                }
                else
                {
                    urlStatsLbl.Content = String.Format("URL: {0}", urlText);
                }

                urlStatsLbl.ToolTip = urlText;
            }
        }

        private void UrlClearBtn_Click(object sender, RoutedEventArgs e)
        {
            urlStatsLbl.Content = "URL:";
            urlTextBox.Text = "";
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
    }
}
