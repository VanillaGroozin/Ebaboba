using Ebabobo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace Ebabobo.Pages
{
    /// <summary>
    /// Логика взаимодействия для TransactionPage.xaml
    /// </summary>
    public partial class TransactionPage : Page
    {

        private Schedule schedule = new Schedule();
        private TransactionType category;
        private int comboBoxFrequency;

        DataTable transactionType = null;
        public TransactionPage()
        {
            InitializeComponent();
            ShowCategoryes();
        }

        private void ShowCategoryes()
        {
            transactionType = new TransactionType().SelectAll();

            //foreach (DataRow dt in transactionType.Rows)
            //{
            //    var cells = dt.ItemArray;
            //    //showCategoryToFront.Add(new TransactionType(Convert.ToInt32(cells[0]), cells[1].ToString(), cells[2].ToString()));
            //}

            if (transactionType != null)
            {
                listOfCategory.DataContext = transactionType.DefaultView;
            }

        }

        private void deleteCategoryBtn(object sender, RoutedEventArgs e)
        {
            if (category != null)
            {
                TransactionType transactionType = new TransactionType(category.TransactionTypeId.ToString());
                if (transactionType != null)
                {
                    transactionType.Delete();
                }
            }
            listOfCategory.DataContext = null;
            ShowCategoryes();

        }

        private void deleteSourceBtn(object sender, RoutedEventArgs e)
        {

        }


        private void addSourceBtn(object sender, RoutedEventArgs e)
        {
            if (comboBoxFrequency == 1)
            {
                schedule.Frequency = "1";
            }
            else if (comboBoxFrequency == 2)
            {
                schedule.Frequency = "2";
            }
            else if (comboBoxFrequency == 3)
            {
                schedule.Frequency = "3";
            }
            else if (comboBoxFrequency == 4)
            {
                schedule.Frequency = "4";
            }

            if (First_date.ToString() != "")
            {
                DateTime dt = Convert.ToDateTime(First_date.ToString());
                string date = dt.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);

                schedule.Date = date;
            }

            schedule.Sum = tb_sourceSum.Text;

            if (MainWindow.CARDID != 0)
            {
                schedule.CardId = MainWindow.CARDID.ToString();
            }


            if (schedule.TypeId == "")
            {
                MessageBox.Show("Выберите категорию.");
            }
            else if (schedule.Date == "")
            {
                MessageBox.Show("Выберите дату");
            }
            else if (schedule.CardId == "")
            {
                MessageBox.Show("Выберите карту");
            }
            else if (schedule.Frequency == "")
            {
                MessageBox.Show("Выберите частоту");
            }
            else if (schedule.Sum == "")
            {
                MessageBox.Show("Введите Сумму");
            }
            else
            {
                schedule.Insert();
                MessageBox.Show("Транзакция добавлена!");
            }

        }

        private void addCategoryBtn(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.Closed += new EventHandler(categoryWindow_Closed);
            categoryWindow.Show();
        }

        void categoryWindow_Closed(object sender, EventArgs e)
        {
            listOfCategory.DataContext = null;
            ShowCategoryes();

        }

        private void listOfCategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView drv = (DataRowView)listOfCategoryDataGrid.SelectedItem;

            if (drv != null)
            {

                var typeId = drv[0].ToString();
                var isIncome = drv.Row[2].ToString();

                if (typeId != null)
                {
                    category = new TransactionType(typeId);
                    schedule.TypeId = typeId;
                    if (isIncome == "True")
                    {
                        schedule.IsIncome = "1";
                    }
                    else
                    {
                        schedule.IsIncome = "0";
                    }
                }
            }

        }

        private void cbFreq_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            comboBoxFrequency = Convert.ToInt32(selectedItem.Tag);
        }

    }
}
