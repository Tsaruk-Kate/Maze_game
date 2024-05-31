using labyrinth_gamе.DataBase.Tables;
using labyrinth_gamе.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

namespace labyrinth_gamе.Views
{
    // Абстрактний клас або інтерфейс для продукту
    public interface IUserFactory
    {
        User CreateUser(string name, string key);
    }

    // Конкретний продукт
    public class UserFactory : IUserFactory
    {
        public User CreateUser(string name, string key)
        {
            return new User
            {
                UserName = name,
                Key = key
            };
        }
    }

    public partial class Labyrinth : Page
    {
        public string RecordName { get; set; }

        // Фабричний метод
        private readonly IUserFactory _userFactory;

        // Конструктор, що приймає фабрику як параметр
        public Labyrinth(IUserFactory userFactory)
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _userFactory = userFactory;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Rules.xaml", UriKind.Relative));
        }

        private void Image_MouseDown_Exit(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(textBoxKey.Text))
            {
                MessageBox.Show("Будь ласка, введіть ім'я користувача та ключ", "Відсутня інформація");
                return;
            }

            string enteredName = textBoxName.Text;
            string namePattern = "^[a-zA-Zа-яА-ЯіІ0-9]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(enteredName, namePattern))
            {
                MessageBox.Show("Ім'я користувача може містити лише українські та англійські букви, а також цифри.",
                    "Помилка");
                return;
            }

            string enteredKey = textBoxKey.Text;
            string keyPattern = "^[0-9]{1,4}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(enteredKey, keyPattern))
            {
                MessageBox.Show("Ключ повинен містити лише цифри і не більше 4 символів.", "Помилка");
                return;
            }

            using var db = new DataBaseContext();
            User? existingUser = db.Users.FirstOrDefault(u => u.UserName == enteredName);
            if (existingUser == null)
            {
                // Використовується фабричний метод для створення нового користувача
                User newUser = _userFactory.CreateUser(enteredName, enteredKey);
                db.Users.Add(newUser);
                db.SaveChanges();
                User.CurrentUser = newUser;
                NavigationService.Navigate(new Levels());
                return;
            }

            if (existingUser.Key != enteredKey)
            {
                MessageBox.Show("Введений ключ невірний", "Помилка");
                return;
            }

            User.CurrentUser = existingUser;
            NavigationService.Navigate(new Levels());
        }
    }
}