using Ebabobo.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ebabobo.Pages
{
    /// <summary>
    /// Логика взаимодействия для IncomeExpensesPage.xaml
    /// </summary>
    public partial class IncomeExpensesPage : Page
    {
        DataTable incomeClone = null;
        DataTable outcomeClone = null;

        private Random randForDate = new Random();
        private Random randForFrequency = new Random();
        private Random randForSum = new Random();
        public IncomeExpensesPage()
        {
            InitializeComponent();
            if (MainWindow.CARDID == 0)
            {
                ShowIncomeMethod("1");
            }
            else
                ShowIncomeMethod(MainWindow.CARDID.ToString());
        }

        public void ShowIncomeMethod(string id)
        {

            DataTable incomeOutcome = new Schedule().SelectByCardId(id);

            var income = from row in incomeOutcome.AsEnumerable()
                         where row.Field<bool>("IsIncome") == true
                         select row;

            incomeClone = incomeOutcome.Clone();
            foreach (DataRow dr in income)
            {
                incomeClone.ImportRow(dr);
            }

            var outcome = from row in incomeOutcome.AsEnumerable()
                          where row.Field<bool>("IsIncome") == false
                          select row;
            outcomeClone = incomeOutcome.Clone();
            foreach (DataRow dr in outcome)
            {
                outcomeClone.ImportRow(dr);
            }

            listOfIncome.DataContext = incomeClone.DefaultView;
            listOfOutcome.DataContext = outcomeClone.DefaultView;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Schedule schedule = new Schedule();
            int cardsCount = new Card().SelectCardsCount();
            Card card = new Card((cardsCount).ToString());

            int typesCount = new TransactionType().SelectTypesCount();
            TransactionType transactionType = new TransactionType((typesCount).ToString());

            DateTime start = new DateTime(2020, 10, 10);
            int range = (DateTime.Today - start).Days;
            start.AddDays(randForDate.Next(range));

            string frequency = randForFrequency.Next(1, 5).ToString();
            string sum = randForSum.Next(501, 10000).ToString();

            schedule.CardId = card.CardId;
            schedule.Date = start.ToString();
            schedule.Frequency = frequency;
            schedule.IsIncome = transactionType.IsIncome;
            schedule.Sum = sum;
            schedule.TypeId = transactionType.TransactionTypeId;

            schedule.Insert();

            ShowIncomeMethod(MainWindow.CARDID.ToString());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Schedule schedule = new Schedule();
            int cardsCount = new Card().SelectCardsCount();
            Card card = new Card((cardsCount).ToString());

            int typesCount = new TransactionType().SelectTypesCount();
            TransactionType transactionType = new TransactionType((typesCount).ToString());

            DateTime start = new DateTime(2020, 10, 10);
            int range = (DateTime.Today - start).Days;
            start.AddDays(randForDate.Next(range));

            string frequency = randForFrequency.Next(1, 5).ToString();
            string sum = randForSum.Next(501, 10000).ToString();

            schedule.CardId = card.CardId;
            schedule.Date = start.ToString();
            schedule.Frequency = frequency;
            schedule.IsIncome = "0";
            schedule.Sum = sum;
            schedule.TypeId = transactionType.TransactionTypeId;

            schedule.Insert();
            ShowIncomeMethod(MainWindow.CARDID.ToString());
        }

    }
}
