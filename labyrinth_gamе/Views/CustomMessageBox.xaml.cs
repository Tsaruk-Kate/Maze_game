using System;
using System.Collections.Generic;
using System.Data;
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

namespace labyrinth_gamе.Views
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message)
        {
            InitializeComponent();
        }
        public static bool? Show(string message)
        {
            return CreateCustomMessageBox(message).ShowDialog();
        }
        private static CustomMessageBox CreateCustomMessageBox(string message)
        {
            var customMessageBox = new CustomMessageBox(message);
            return customMessageBox;
        }
        private void Button_Home(object sender, RoutedEventArgs e)
        {
            // Створення екземпляру фабрики користувачів
            IUserFactory userFactory = new UserFactory();
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            // Передача фабрики користувачів у конструктор Labyrinth
            frame.Navigate(new Labyrinth(new UserFactory()));
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
        private void Button_Levels(object sender, RoutedEventArgs e)
        {
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            frame.Navigate(new Levels());
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
    }
}
