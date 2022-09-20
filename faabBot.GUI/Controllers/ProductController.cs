using faabBot.GUI.EventArguments;
using faabBot.GUI.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpenQA.Selenium.DevTools;
using System;
using System.Linq;

namespace faabBot.GUI.Controllers
{
    public class ProductController
    {
        private readonly MainWindow _mainWindow;
        public event EventHandler<ProductEventArgs>? NewProductAdded;
        public ObservableHashSet<Product> ProductQueue { get; set; } = new();

        public ProductController(MainWindow mainWindow, LogController log)
        {
            _mainWindow = mainWindow;
        }

        public void NewProductAddedEvent(Product newProduct)
        {
            ProductEventArgs args = new()
            {
                Product = newProduct
            };

            OnNewProductAddedEvent(args);
        }

        void ProductController_LogMessage(object? sender, LogEventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.LogInstance.Log(e.Message!, e.Created);
            });
        }

        public void AddNewProduct(Product newProduct)
        {
            if (ProductQueue.FirstOrDefault(p => p.Url == newProduct.Url) == default(Product))
            {
                var maxIndex = ProductQueue.Count > 0 ? ProductQueue.Max(p => p.Id) : 0;
                newProduct.Id = maxIndex + 1;
                ProductQueue.Add(newProduct);

                _mainWindow.productsListBox.SelectedIndex = _mainWindow.productsListBox.Items.Count - 1;
                _mainWindow.productsListBox.ScrollIntoView(_mainWindow.productsListBox.SelectedItem);
            }
            else
            {
                //_log.NewLogCreatedEvent("Product not added... A product with identical URL already exists", DateTime.Now);
            }
        }

        protected virtual void OnNewProductAddedEvent(ProductEventArgs @event)
        {
            NewProductAdded?.Invoke(this, @event);
        }
    }
}
