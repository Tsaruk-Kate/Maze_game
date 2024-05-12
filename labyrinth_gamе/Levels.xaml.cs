using labyrinth_gamе.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace labyrinth_gamе
{
    public partial class Levels : Page
    {
        public Levels()
        {
            InitializeComponent();
        }
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Labyrinth(new UserFactory()));
        }
        private void Image_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Results.xaml", UriKind.Relative));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = Application.Current.MainWindow;
            Level_1 windowLevel1 = new Level_1();
            windowLevel1.Show();
            window.Close();
        }
    }
}
