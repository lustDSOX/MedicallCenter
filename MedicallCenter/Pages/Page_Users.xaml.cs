using MedicallCenter;
using MedicallCenter.Clasees;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

namespace MedicalCenter.Pages
{
    /// <summary>
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Page_Users : Page
    {
        private int pageNumber = 0;
        private int maxpage = 0;
        private int pageSize = 20;
        List<User> users = new List<User>();
        User currentuser = new User();

        public Page_Users()
        {
            InitializeComponent();
            if (CurrentData.worker.Type1.id != 1)
            {
                btn_DeleteUser.Visibility = Visibility.Hidden;
                btn_AddEditUser.Visibility = Visibility.Hidden;
            }

            users = CurrentData.db.User.ToList();
            maxpage = users.Count / pageSize;
            DisplayDataInGrid();
        }

        private void DisplayDataInGrid()
        {
            var currentPageData = users.Skip(pageNumber * pageSize).Take(pageSize); // отображаем только данные для текущей страницы
            DataGridUser.ItemsSource = currentPageData; // отображаем данные в DataGrid
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber < maxpage)
            {
                pageNumber++; // переход на следующую страницу
                DisplayDataInGrid(); // отображение данных
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber > 0)
            {
                pageNumber--; // переход на предыдущую страницу
                DisplayDataInGrid(); // отображение данных
            }
        }

        private void search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (search.Text == "Поиск")
                search.Text = "";
            else if (search.Text == "")
                search.Text = "Поиск";
        }
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (search.Text != "" && search.Text != "Поиск" && DataGridUser != null)
            {
                string s_text = search.Text.ToLower();
                var u1 = users.Where(n => n.name.ToLower().Contains(s_text)).ToList();
                var u2 = users.Where(n => n.age.ToString().ToLower().Contains(s_text)).ToList();
                var u3 = users.Where(n => n.login.ToLower().Contains(s_text)).ToList();
                users = u1.Concat(u2.Concat(u3)).ToList();
                DisplayDataInGrid();
            }
            else
            {
                if (DataGridUser != null)
                {
                    users = CurrentData.users;
                    DisplayDataInGrid();
                }

            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            User user = DataGridUser.SelectedValue as User;
            if (CurrentData.worker.Type1.id == 1)
                Manager.frame.Navigate(new Page_UsersAddEdit(user));
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.frame.Navigate(new Page_Home(CurrentData.worker));
        }

        private void btnAddEditUser_Click(object sender, RoutedEventArgs e)
        {
            Manager.frame.Navigate(new Page_UsersAddEdit(currentuser));
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var userForDelete = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {userForDelete} записи", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    CurrentData.db.User.RemoveRange(userForDelete);
                    CurrentData.db.SaveChanges();
                    DataGridUser.ItemsSource = CurrentData.db.User.ToList();
                    //DataGridResult.ItemsSource = CurrentData.results;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
