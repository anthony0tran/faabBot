using faabBot.GUI.Controllers;
using faabBot.GUI.Validators;
using System;
using System.Windows;
using System.Windows.Data;

namespace faabBot.GUI.Views
{
    /// <summary>
    /// Interaction logic for AddSizeWindow.xaml
    /// </summary>
    public partial class AddSizeWindow : Window
    {
        public bool CustomSize { get; set; } = false;
        private SizeController SizesInstance { get; set; }
        private readonly ListCollectionView SizesCollection;

        public AddSizeWindow(SizeController sizesInstance)
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
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddSizeValidator.ValidateInputFields(this))
            {
                if (CustomSize)
                {
                    if (!string.IsNullOrEmpty(AddSizeTextBox.Text) && AddSizeValidator.IsNotAdded(AddSizeTextBox.Text, SizesInstance.Sizes))
                    {
                        SizesInstance.Sizes.Add(AddSizeTextBox.Text);

                        if (SizesInstance.Sizes.Contains("ALL AVAILABLE SIZES"))
                        {
                            SizesInstance.Sizes.Remove("ALL AVAILABLE SIZES");
                        }

                        Close();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AddSizeComboBox.SelectedItem.ToString()) && AddSizeComboBox.SelectedItem != null)
                    {
                        if (AddSizeValidator.IsNotAdded(AddSizeComboBox.SelectedItem.ToString()!, SizesInstance.Sizes))
                        {
                            SizesInstance.Sizes.Add(AddSizeComboBox.SelectedItem.ToString()!);

                            if (SizesInstance.Sizes.Contains("ALL AVAILABLE SIZES"))
                            {
                                SizesInstance.Sizes.Remove("ALL AVAILABLE SIZES");
                            }

                            Close();
                        }

                    }
                }
            }
        }
    }
}
