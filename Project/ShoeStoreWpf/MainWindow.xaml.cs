using System.Windows;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ShoeStoreWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Аутентифицикация пользователя по логину и паролю из бд
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new ShoeStoreDbContext();
            var user = context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == LoginTextBox.Text && u.PasswordHash == PasswordBox.Password);
            if (user != null)
            {
                var productsWindow = new ProductsWindow(user);
                productsWindow.Show();
                Close();
            }
            else
                MessageBox.Show("Неверный логин или пароль.");
        }

        /// <summary>
        /// Открывает окно товаров в режиме гостя
        /// </summary>
        private void GuestLogin_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsWindow(null);
            productsWindow.Show();
            Close();
        }
    }
}
