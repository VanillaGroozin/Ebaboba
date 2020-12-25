using Ebabobo.Models;
using Ebabobo.Pages;
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

namespace Ebabobo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object OperationsPage = new OperationsPage();
        private object IncomeExpensesPage = new IncomeExpensesPage();
        private object InfoPage = new InfoPage();
        private object TransactionPage = new TransactionPage();

        private DataTable cards = null;
        public static int CARDID = 0;
        public MainWindow()
        {
            InitializeComponent();
            FillCardGrid();

            InfoFrame.Navigate(InfoPage);

            AutomaticIncomeSchedule();
            ShowCards();
        }

        //back Календарная Автоматика
        //2 беру все  данные с schedule и пробегаю в цикле
        //3 вычисляю дни по Date
        //4 закидываю деньги на кошелек по cardId
        //5 записываю в историю ид-карты, даты, инком-тру, сумму и тип
        //6 меняю дату в schedule Date
        //7 при входе в программу запускаю скрипт в отдельном потоке
        //8 готово

        private void CardIncomeUpdate(string cardId, double sum)
        {
            Card card = new Card(cardId);
            card.Sum = (double.Parse(card.Sum) + sum).ToString();
            card.Update();
        }

        private void CardOutcomeUpdate(string cardId, double sum)
        {
            Card card = new Card(cardId);
            card.Sum = (double.Parse(card.Sum) - sum).ToString();
            card.Update();
        }

        private void InsertNewHistory(string cardId, string date, string isIncome, string sum, string type, string currencyId)
        {
            History history = new History();

            history.CardId = cardId;
            history.Date = date;
            if (isIncome == "True")
            {
                history.IsIncome = "1";
            }
            else
            {
                history.IsIncome = "0";
            }
            history.Sum = sum;
            history.TypeId = type;
            history.CurrencyId = currencyId;

            history.Insert();
        }

        private void UpdateSheduleDate(string id)
        {
            Schedule schedule = new Schedule(id);

            DateTime dateTime = DateTime.Now;
            string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
            schedule.Date = date;
            schedule.Sum = double.Parse(schedule.Sum).ToString();

            schedule.Update();
            schedule = null;
        }


        private double DoNoteRepeatYourSelf(int monthOrWeekOrDay, double schedulIncomeSum)
        {
            return schedulIncomeSum * monthOrWeekOrDay;
        }

        private void AutomaticIncomeSchedule()
        {

            double sum = 0;

            DataTable allSchelulesByCardId = new Schedule().SelectAll();

            foreach (DataRow drSchedule in allSchelulesByCardId.Rows)
            {
                var scheduleCells = drSchedule.ItemArray;

                var lastScheduleDate = scheduleCells[2];

                if (lastScheduleDate != null)
                {
                    //Last Date
                    DateTime scheduleDate = Convert.ToDateTime(lastScheduleDate.ToString());

                    if (DateTime.Compare(DateTime.Now, scheduleDate) > 0)
                    {
                        TimeSpan difference = (scheduleDate - DateTime.Now).Duration();

                        if (difference.Days >= 365 && Convert.ToInt32(scheduleCells[3]) == 4)
                        {

                            if (scheduleCells[6].ToString() == "True")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardIncomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);

                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                            else if (scheduleCells[6].ToString() == "False")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardOutcomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }

                        }
                        else if (difference.Days > 30 && Convert.ToInt32(scheduleCells[3]) == 3)
                        {
                            if (scheduleCells[6].ToString() == "True")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardIncomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                            else if (scheduleCells[6].ToString() == "False")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardOutcomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                        }
                        else if (difference.Days >= 7 && Convert.ToInt32(scheduleCells[3]) == 2)
                        {
                            if (scheduleCells[6].ToString() == "True")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 7), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardIncomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                            else if (scheduleCells[6].ToString() == "False")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardOutcomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                        }
                        else if (difference.Days >= 1 && Convert.ToInt32(scheduleCells[3]) == 1)
                        {

                            if (scheduleCells[6].ToString() == "True")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardIncomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                            else if (scheduleCells[6].ToString() == "False")
                            {
                                sum = DoNoteRepeatYourSelf((difference.Days / 30), double.Parse(scheduleCells[4].ToString()));

                                //Update Card by cardid sum
                                CardOutcomeUpdate(scheduleCells[1].ToString(), sum);

                                //entry into the db new History
                                DateTime dateTime = Convert.ToDateTime(lastScheduleDate.ToString());
                                string date = dateTime.ToString("M/dd/yyyy", CultureInfo.InvariantCulture);
                                InsertNewHistory(scheduleCells[1].ToString(), date, scheduleCells[6].ToString(), sum.ToString(), scheduleCells[5].ToString(), new Card(scheduleCells[1].ToString()).CurrencyId.ToString());

                                //Update schedule date by id
                                UpdateSheduleDate(scheduleCells[0].ToString());
                            }
                        }
                    }
                }

            }

        }

        public void ShowCards()
        {
            cards = new Card().SelectAll();

            if (cards != null)
            {
                listOfCards.DataContext = cards.DefaultView;

            }
        }


        private void FillCardGrid()
        {
            var dt = new Card().SelectAll();
            listOfCards.DataContext = dt.DefaultView;
        }
        private void IncomeExpenses_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IncomeExpenses.IsSelected)
            {
                IncomeExpensesFrame.Navigate(IncomeExpensesPage);
            }
        }

        private void Info_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Info.IsSelected)
            {
                InfoFrame.Navigate(InfoPage);
            }
        }

        private void Operations_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Operations.IsSelected)
            {
                OperationsFrame.Navigate(OperationsPage);
            }
        }

        private void Transaction_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Transaction.IsSelected)
            {
                TransactionFrame.Navigate(TransactionPage);
            }
        }

        private void createCardBtn(object sender, RoutedEventArgs e)
        {
            CreateAccount window = new CreateAccount();
            window.Closed += new EventHandler(window1_Closed);
            window.Show();
        }

        void window1_Closed(object sender, EventArgs e)
        {
            listOfCards.DataContext = null;
            ShowCards();
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {

        }

        private void listOfOperationsGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void listOfOperationsGV_Selected(object sender, MouseButtonEventArgs e)
        {
            DataRowView card = (DataRowView)listOfOperationsGV.SelectedItem;

            var dr = card.Row[0];
            CARDID = Convert.ToInt32(dr);

            InfoPage = new InfoPage(CARDID);
            IncomeExpensesPage = new IncomeExpensesPage();
        }
    }
}
