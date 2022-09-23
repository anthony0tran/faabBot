using faabBot.GUI.EventArguments;
using faabBot.GUI.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace faabBot.GUI.Controllers
{
    public class ProductController
    {
        private readonly MainWindow _mainWindow;
        public event EventHandler<ProductEventArgs>? NewProductAdded;
        private readonly LogController _log;
        public ObservableCollection<Product> ProductQueue { get; set; } = new();

        public ProductController(MainWindow mainWindow)
        {
            _log = new(mainWindow);
            _mainWindow = mainWindow;
            _log.NewLogCreated += ProductController_LogMessage;
        }

        void ProductController_LogMessage(object? sender, LogEventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.LogInstance.Log(e.Message!, e.Created);
            });
        }

        public void NewProductAddedEvent(Product newProduct)
        {
            ProductEventArgs args = new()
            {
                Product = newProduct
            };

            OnNewProductAddedEvent(args);
        }

        public void AddNewProduct(Product newProduct)
        {
            if (newProduct.Url == null)
            {
                return;
            }

            var productId = GetProductIdFromUrl(newProduct.Url);

            if (ProductQueue.FirstOrDefault(p => p.ProductId == productId) == default(Product))
            {
                var maxIndex = ProductQueue.Count > 0 ? ProductQueue.Max(p => p.Id) : 0;
                newProduct.Id = maxIndex + 1;
                newProduct.ProductId = productId;
                ProductQueue.Add(newProduct);

                _mainWindow.productsListBox.SelectedIndex = _mainWindow.productsListBox.Items.Count - 1;
                _mainWindow.productsListBox.ScrollIntoView(_mainWindow.productsListBox.SelectedItem);
            }
            else
            {
                _log.NewLogCreatedEvent(string.Format("Product not added... {0} already added", GetProductIdFromUrl(newProduct.Url)), DateTime.Now);
            }
        }

        public void RemoveProduct(Product product)
        {
            ((ObservableCollection<Product>)_mainWindow.productsListBox.ItemsSource).Remove(product);
            _mainWindow.productsListBox.SelectedIndex = _mainWindow.productsListBox.Items.Count - 1;
            _mainWindow.productsListBox.ScrollIntoView(_mainWindow.productsListBox.SelectedItem);
        }

        private static int GetProductIdFromUrl(string url)
        {
            var saleSubString = "/goods-sale/";
            var normalSubString = "/goods/";
            var id = "";

            if (url.Contains(saleSubString))
            {
                var stringIndex = url.LastIndexOf(saleSubString) + saleSubString.Length;
                var urlSubstring = url.Substring(stringIndex);

                char[] chars = urlSubstring.ToCharArray();

                foreach (char c in chars)
                {
                    if (char.IsDigit(c))
                    {
                        id += c;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (url.Contains(normalSubString))
            {
                var stringIndex = url.LastIndexOf(normalSubString) + normalSubString.Length;
                var urlSubstring = url.Substring(stringIndex);

                char[] chars = urlSubstring.ToCharArray();

                foreach (char c in chars)
                {
                    if (char.IsDigit(c))
                    {
                        id += c;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (int.TryParse(id, out var idResult))
            {
                return idResult;
            }

            return 0;
        }

        protected virtual void OnNewProductAddedEvent(ProductEventArgs @event)
        {
            NewProductAdded?.Invoke(this, @event);
        }
    }
}
