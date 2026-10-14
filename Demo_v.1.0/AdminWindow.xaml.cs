using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Demo_v._1._0
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public User NewUser { get; private set; }
        public ObservableCollection<User> Users { get; set; }

        public AdminWindow()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            LoadUsers();
            DataGridUsers.ItemsSource = Users;
        }
        private void LoadUsers()
        {
            using (var context = new DemoDBEntities())
            {
                var usersFromDb = context.Users.ToList();
                Users.Clear();
                foreach (var user in usersFromDb)
                {
                    Users.Add(user);
                }
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string lastname = txtLastname.Text.Trim();
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = txtRole.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Введите полные данные пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.NewUser = new User
            {
                name = name.Trim(),
                lastname = lastname.Trim(),
                login = login.Trim(),
                password = password.Trim(),
                role = role,
                IsBlocked = false,
            };

            try
            {
                using (var context = new DemoDBEntities())
                {
                    context.Users.Add(this.NewUser);
                    context.SaveChanges();
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            var user = DataGridUsers.SelectedItem as User;

            if (user == null)
            {
                MessageBox.Show("Выберите пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new DemoDBEntities())
                {

                    var userToDelete = context.Users.Find(user.id);

                    if (userToDelete != null)
                    {
                        context.Users.Remove(userToDelete);
                        context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                Users.Remove(user);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
}
