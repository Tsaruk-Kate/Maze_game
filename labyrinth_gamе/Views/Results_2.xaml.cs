using labyrinth_gamе.DataBase;
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
    /// <summary>
    /// Interaction logic for Results_2.xaml
    /// </summary>
    public partial class Results_2 : Page
    {
        public Results_2()
        {
            InitializeComponent();
            using (var db = new DataBaseContext())
            {
                recordsListBox.ItemsSource = db.Records.Where(r => r.Level == 2).OrderBy(r => r.Time).ToList();
            }
        }

        private void Image_MouseDown_ArrowR(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Results_3.xaml", UriKind.Relative));
        }
        private void Image_MouseDown_ArrowL(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Results.xaml", UriKind.Relative));
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DataBaseContext())
            {
                var records = db.Records.Where(r => r.Level == 2).ToList();
                db.Records.RemoveRange(records);
                db.SaveChanges();
                recordsListBox.ItemsSource = null;
            }
        }
    }
}
