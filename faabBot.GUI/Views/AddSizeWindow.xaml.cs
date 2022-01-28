using faabBot.GUI.Controllers;
using faabBot.GUI.Helpers;
using faabBot.GUI.Validators;
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
using System.Windows.Shapes;

namespace faabBot.GUI.Views
{
    /// <summary>
    /// Interaction logic for AddSizeWindow.xaml
    /// </summary>
    public partial class AddSizeWindow : Window
    {
        public bool CustomSize { get; set; } = false;
        private SizesController SizesInstance { get; set; }
        private readonly CollectionView SizesCollection;

        public AddSizeWindow(SizesController sizesInstance)
        {
            InitializeComponent();

            SizesInstance = sizesInstance;

            SizesCollection = new(Globals.Sizes);

            AddSizeComboBox.ItemsSource = SizesCollection;

        }

        private void CustomSizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CustomSize = true;
        }

        private void CustomSizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!AddSizeComboBox.IsEnabled && CustomSize)
            {
                AddSizeComboBox.IsEnabled = true;
                AddSizeTextBox.IsEnabled = false;
                CustomSize = false;
                AddSizeTextBox.Text = default;
            }
            else
            {
                AddSizeComboBox.IsEnabled = false;
                AddSizeComboBox.SelectedIndex = default;
                AddSizeTextBox.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddSizeValidator.ValidateInputFields(this))
            {
                if (CustomSize)
                {
                    if (!String.IsNullOrEmpty(AddSizeTextBox.Text) && AddSizeValidator.isNotAdded(AddSizeTextBox.Text, SizesInstance.Sizes))
                    {
                        SizesInstance.Sizes.Add(AddSizeTextBox.Text);
                        this.Close();
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(AddSizeComboBox.SelectedItem.ToString()) && AddSizeComboBox.SelectedItem != null)
                    {
                        if (AddSizeValidator.isNotAdded(AddSizeComboBox.SelectedItem.ToString()!, SizesInstance.Sizes))
                        {
                            SizesInstance.Sizes.Add(AddSizeComboBox.SelectedItem.ToString()!);
                            this.Close();
                        }

                    }
                }
            }
        }
    }
}
