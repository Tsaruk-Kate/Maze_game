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

namespace labyrinth_gamе.Views
{
    public partial class Rules : Page
    {
        public Rules()
        {
            InitializeComponent();
        }
        private void Image_MouseDown_6(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Labyrinth(new UserFactory()));
        }
        private void Image_MouseDown_ArrowR(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Rules_2.xaml", UriKind.Relative));
        }
    }
}
