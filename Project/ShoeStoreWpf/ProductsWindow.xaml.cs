using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System.Windows;


namespace ShoeStoreWpf
{
    public partial class ProductsWindow : Window
    {
        private readonly User _currentUser;
        private List<ProductViewModel> _allProducts = new();

        /// <summary>
        /// Сохранение пользователя, отображение имени, скрытие панели доступной Администратору и Менеджеру
        /// </summary>
        public ProductsWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // Имя пользователя или "Гость"
            if (_currentUser != null)
            {
                UserFullName.Text = $"{_currentUser.SecondName} {_currentUser.FirstName} {_currentUser.MiddleName}".Trim();
                if (_currentUser.Role?.Name == "Администратор" || _currentUser.Role?.Name == "Менеджер")
                    AdminPanel.Visibility = Visibility.Visible; 
            }
            else
                UserFullName.Text = "Гость";
        }

        /// <summary>
        /// Загрузка товаров из бд, заполнение ComboBox производителей 
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var context = new ShoeStoreDbContext();

            _allProducts = context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    Article = p.Article,
                    CategoryName = p.Category.Name,
                    Unit = p.Unit ?? "шт.",
                    Price = p.Price,
                    SupplierName = p.Supplier.Name,
                    ManufacturerName = p.Manufacturer.Name,
                    ManufacturerId = p.ManufacturerId,
                    Discount = p.Discount ?? 0,
                    Quantity = p.Quantity,
                    Gender = p.Gender ?? "N/A",
                    Description = p.Description ?? "",
                    Color = p.Color ?? "",
                    Size = p.Size ?? 0,
                    Image = p.Image ?? "picture.png",
                    OriginalPrice = p.Price.ToString("F2"),
                    FinalPrice = (p.Price * (1 - (p.Discount ?? 0) / 100m)).ToString("F2"),
                    HasDiscount = p.Discount > 0,
                    Background = p.Discount > 15 ? "#2E8B57" : (p.Quantity == 0 ? "LightBlue" : "Transparent")
                }).ToList();

            var manufacturers = _allProducts.Select(p => p.ManufacturerName).Distinct().OrderBy(m => m).ToList();
            manufacturers.Insert(0, "Все");
            ManufacturerComboBox.ItemsSource = manufacturers;
            ManufacturerComboBox.SelectedIndex = 0;

            SortComboBox.SelectedIndex = 0;

            ProductsListBox.ItemsSource = _allProducts;

            ApplyFilters(null, null);
        }

        /// <summary>
        /// Применение фильтров, поиска и сортировки, игнорирование регистра
        /// </summary>
        private void ApplyFilters(object sender, EventArgs e)
        {
            var filtered = _allProducts.AsQueryable();

            // Поиск по описанию
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var search = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(p => p.Description.ToLower().Contains(search));
            }

            // Фильтрация по производителю
            if (ManufacturerComboBox.SelectedItem is string selectedMan && selectedMan != "Все")
                filtered = filtered.Where(p => p.ManufacturerName == selectedMan);

            // Фильтрация по максимальной цене
            if (decimal.TryParse(MaxPriceTextBox.Text, out var maxPrice))
                filtered = filtered.Where(p => decimal.Parse(p.FinalPrice) <= maxPrice);

            // Фильтрация товаров со скидкой
            if (OnlyDiscountedCheckBox.IsChecked == true)
                filtered = filtered.Where(p => p.HasDiscount);

            // Фильтрация товаров в наличии
            if (OnlyInStockCheckBox.IsChecked == true)
                filtered = filtered.Where(p => p.Quantity > 0);

            // Сортировка
            switch (SortComboBox.SelectedIndex)
            {
                case 0: 
                    filtered = filtered.OrderBy(p => p.CategoryName); 
                    break;
                case 1: 
                    filtered = filtered.OrderBy(p => p.SupplierName); 
                    break;
                case 2: 
                    filtered = filtered.OrderBy(p => decimal.Parse(p.FinalPrice)); 
                    break;
                case 3: 
                    filtered = filtered.OrderByDescending(p => decimal.Parse(p.FinalPrice)); 
                    break;
                default: 
                    filtered = filtered.OrderBy(p => p.CategoryName); 
                    break;
            }

            ProductsListBox.ItemsSource = filtered.ToList();
        }

        /// <summary>
        /// Добавление товара
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
            => OpenEditWindow(null);

        /// <summary>
        /// Редактирование
        /// </summary>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is ProductViewModel selectedVm)
            {
                OpenEditWindow(selectedVm);
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        /// <summary>
        /// Удаление товара
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsListBox.SelectedItems.Cast<ProductViewModel>().ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Выберите товары для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selected.Count} товар(ов)?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var context = new ShoeStoreDbContext();
                foreach (var vm in selected)
                {
                    var product = context.Products.Find(vm.ProductId);
                    if (product != null) context.Products.Remove(product);
                }
                context.SaveChanges();
                Window_Loaded(null, null);
            }
        }

        /// <summary>
        /// Открытие окна редактирования и обновление списка
        /// </summary>
        private void OpenEditWindow(ProductViewModel vm)
        {
            new EditProductWindow(vm).ShowDialog();
            Window_Loaded(null, null);
        }
    }
}

