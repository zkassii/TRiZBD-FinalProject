using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System.IO;
using System.Windows;
using System;
using System.Linq;

namespace ShoeStoreWpf
{
    public partial class EditProductWindow : Window
    {
        private readonly ProductViewModel _product;
        private string _imageFileName;

        /// <summary>
        /// Загрузка данных в поля при редактировании
        /// </summary>
        public EditProductWindow(ProductViewModel product)
        {
            InitializeComponent();
            _product = product ?? new ProductViewModel();
            LoadCombos();

            // Установка значений по умолчанию для новых товаров
            if (_product.ProductId == 0)
                UnitTextBox.Text = "шт.";

            if (product != null)
            {
                ArticleTextBox.Text = _product.Article;
                CategoryComboBox.SelectedItem = _product.CategoryName;
                UnitTextBox.Text = _product.Unit;
                PriceTextBox.Text = _product.Price.ToString();
                SupplierComboBox.SelectedItem = _product.SupplierName;
                ManufacturerComboBox.SelectedItem = _product.ManufacturerName;
                DiscountTextBox.Text = _product.Discount.ToString();
                QuantityTextBox.Text = _product.Quantity.ToString();
                GenderTextBox.Text = _product.Gender;
                DescriptionTextBox.Text = _product.Description;
                ImagePathTextBlock.Text = _product.Image;
                _imageFileName = _product.Image;
            }
        }

        /// <summary>
        /// Загружает ComboBox из бд
        /// </summary>
        private void LoadCombos()
        {
            using var context = new ShoeStoreDbContext();
            CategoryComboBox.ItemsSource = context.Categories.Select(c => c.Name).ToList();
            SupplierComboBox.ItemsSource = context.Suppliers.Select(s => s.Name).ToList();
            ManufacturerComboBox.ItemsSource = context.Manufacturers.Select(m => m.Name).ToList();

            // Выбор по умолчанию
            if (CategoryComboBox.Items.Count > 0) 
                CategoryComboBox.SelectedIndex = 0;

            if (SupplierComboBox.Items.Count > 0) 
                SupplierComboBox.SelectedIndex = 0;

            if (ManufacturerComboBox.Items.Count > 0) 
                ManufacturerComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузка изображения
        /// </summary>
        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Images|*.jpg;*.png" };
            if (openFileDialog.ShowDialog() == true)
            {
                _imageFileName = Path.GetFileName(openFileDialog.FileName);
                File.Copy(openFileDialog.FileName, $"Images/{_imageFileName}", true);
                ImagePathTextBlock.Text = _imageFileName;
            }
        }

        /// <summary>
        /// Сохранение
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка ComboBox
            if (CategoryComboBox.SelectedItem == null || SupplierComboBox.SelectedItem == null || ManufacturerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию, поставщика и производителя");
                return;
            }

            // Проверка чисел
            if (!int.TryParse(PriceTextBox.Text, out int price) ||!byte.TryParse(DiscountTextBox.Text, out byte discount) ||!int.TryParse(QuantityTextBox.Text, out int quantity) ||!byte.TryParse(SizeTextBox.Text, out byte size))
            {
                MessageBox.Show("Проверьте: цена, скидка, количествово, размер");
                return;
            }

            using var context = new ShoeStoreDbContext();
            var category = context.Categories.First(c => c.Name == (string)CategoryComboBox.SelectedItem);
            var supplier = context.Suppliers.First(s => s.Name == (string)SupplierComboBox.SelectedItem);
            var manufacturer = context.Manufacturers.First(m => m.Name == (string)ManufacturerComboBox.SelectedItem);

            var product = _product.ProductId > 0 ? context.Products.Find(_product.ProductId) : new Product();

            //Заполнение или обновление товара
            product.Article = ArticleTextBox.Text;
            product.CategoryId = category.CategoryId;
            product.Unit = UnitTextBox.Text;
            product.Price = price;
            product.SupplierId = supplier.SupplierId;
            product.ManufacturerId = manufacturer.ManufacturerId;
            product.Discount = discount;
            product.Quantity = quantity;
            product.Gender = GenderTextBox.Text;
            product.Description = DescriptionTextBox.Text;
            product.Color = ColorTextBox.Text;
            product.Size = size;
            product.Image = _imageFileName ?? "picture.png";

            if (_product.ProductId == 0) context.Products.Add(product);
            context.SaveChanges();
            Close();
        }
    }
}