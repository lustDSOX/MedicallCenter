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
    /// Логика взаимодействия для Result.xaml
    /// </summary>
    public partial class Page_Result : Page
    {
        private Result currentresult = new Result();

        private int pageNumber = 0;
        private int maxpage = 0;
        private int pageSize = 20;
        List<Result> results = new List<Result>();

        public Page_Result()
        {
            InitializeComponent();
            if (CurrentData.worker.Type1.id != 1)
            {
                bntAddResult.Visibility = Visibility.Hidden;
                bntDeleteResult.Visibility = Visibility.Hidden;
            }
            //results = CurrentData.results;
            results = CurrentData.db.Result.ToList();
            maxpage = results.Count / pageSize;
            DisplayDataInGrid();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.frame.Navigate(new Page_Home(CurrentData.worker));
        }

        private void DisplayDataInGrid()
        {
            //DataGridResult.ItemsSource = CurrentData.results;
            DataGridResult.ItemsSource = CurrentData.db.Result.ToList();
            var currentPageData = results.Skip(pageNumber * pageSize).Take(pageSize); // отображаем только данные для текущей страницы
            DataGridResult.ItemsSource = currentPageData; // отображаем данные в DataGrid
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
            if (search.Text != "" && search.Text != "Поиск" && DataGridResult != null)
            {
                string s_text = search.Text.ToLower();
                var s = results.Where(n => n.User.name.ToLower().Contains(s_text)).ToList();
                var s1 = results.Where(n => n.date.ToLower().Contains(s_text)).ToList();
                var s2 = results.Where(n => n.result1.ToString().ToLower().Contains(s_text)).ToList();
                var s3 = results.Where(n => n.Worker.name.ToString().ToLower().Contains(s_text)).ToList();
                var s4 = results.Where(n => n.Service.service1.ToString().ToLower().Contains(s_text)).ToList();
                results = s1.Concat(s3.Concat(s2.Concat(s.Concat(s4)))).ToList();
                DisplayDataInGrid();
            }
            else
            {
                if (DataGridResult != null)
                {
                    results = CurrentData.results;
                    DisplayDataInGrid();
                }

            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentData.worker.Type1.id == 1)
            {
                Result result = DataGridResult.SelectedValue as Result;

                Manager.frame.Navigate(new Page_ResultAddEdit(result));
            }          
        }

        private void btnDeleteResult_Click(object sender, RoutedEventArgs e)
        {
            var resultForDelete = DataGridResult.SelectedItems.Cast<Result>().ToList();
            if(MessageBox.Show($"Вы точно хотите удалить следующие {resultForDelete} записи","Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question)== MessageBoxResult.Yes)
            {
                try
                {
                    CurrentData.db.Result.RemoveRange(resultForDelete);
                    CurrentData.db.SaveChanges();
                    DataGridResult.ItemsSource = CurrentData.db.Result.ToList();
                    //DataGridResult.ItemsSource = CurrentData.results;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btnEditResult_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridResult.SelectedItem != null)
                currentresult = DataGridResult.SelectedItem as Result;
            Manager.frame.Navigate(new Page_ResultAddEdit(currentresult));
        }

        private void bntAddResult_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridResult.SelectedItem != null)
                currentresult = new Result();
            Manager.frame.Navigate(new Page_ResultAddEdit(currentresult));
        }
    }
}
