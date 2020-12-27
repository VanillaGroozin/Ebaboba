using Ebabobo.Models;
using Ebabobo.Pages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
            FillTransactionCards();
            InfoFrame.Navigate(InfoPage);

            AutomaticIncomeSchedule();
            ShowCards();
        }

        private void FillTransactionCards()
        {
            FillByAllCards(cbFromCard);
            FillByAllCards(cbToCard);
        }

        private void FillByAllCards(ComboBox cardCb)
        {
            var dt = new Card().SelectAll();
            FillCardComboBox(cardCb, dt);
        }

        private void FillCardComboBox(ComboBox cardCb, DataTable dt)
        {
            cardCb.ItemsSource = dt.DefaultView;
            cardCb.DisplayMemberPath = "Name";
            cardCb.SelectedValuePath = "CardId";
        }

        private void cbFromCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cardFrom = new Card(cbFromCard.SelectedValue.ToString());
            tbCurrencyOfCard.Text = new Currency(cardFrom.CurrencyId).Name;
            FillCardComboBox(cbToCard, new Card().SelectAllByCurrency(cardFrom.CurrencyId));
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            try
            {                
                if (cbFromCard.SelectedValue.Equals(cbToCard.SelectedValue))
                    throw new Exception("Выбраны одинаковые карты!");

                double sumToSend = 0;
                if (!double.TryParse(tbSum.Text.Replace('.', ','), out sumToSend))
                    throw new Exception("Неверное значение суммы!");

                var cardFrom = new Card(cbFromCard.SelectedValue.ToString());
                if (double.TryParse(cardFrom.Sum, out var sum))
                {
                    if (sum - sumToSend >= 0)
                    {
                        var cardTo = new Card(cbToCard.SelectedValue.ToString());
                        using (TransactionScope scope = new TransactionScope())
                        {                          
                            cardFrom.Sum = (sum - sumToSend).ToString();
                            cardTo.Sum = (Double.Parse(cardTo.Sum) + sumToSend).ToString();
                            cardFrom.Update();
                            cardTo.Update();
                            
                            MessageBox.Show("Перевод завершен!");
                            scope.Complete();                           
                        }
                        var historyFrom = new History();                     
                        historyFrom.CardId = cardFrom.CardId;
                        historyFrom.Sum = sumToSend.ToString();
                        historyFrom.CurrencyId = cardFrom.CurrencyId;
                        historyFrom.Date = DateTime.Now.ToString();
                        historyFrom.IsIncome = "0";
                        historyFrom.TypeId = "0";

                        var historyTo = new History();
                        historyTo.CardId = cardTo.CardId;
                        historyTo.Sum = sumToSend.ToString();
                        historyTo.CurrencyId = cardTo.CurrencyId;
                        historyTo.Date = DateTime.Now.ToString();
                        historyTo.IsIncome = "1";
                        historyTo.TypeId = "0";

                        historyFrom.Insert();
                        historyTo.Insert();
                        FillCardGrid();
                    } 
                    else 
                        throw new Exception("Недостаточно средств!");
                }
                else
                    throw new Exception($"Ошибка карты {cardFrom.Name}!");        
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //back Календарная Автоматика
        //2 беру все  данные с schedule и пробегаю в цикле
        //3 вычисляю дни по Date
        //4 закидываю деньги на кошелек по cardId
        //5 записываю в историю ид-карты, даты, инком-тру, сумму и тип
        //6 меняю дату в schedule Date
        //7 при входе в программу запускаю скрипт в отдельном потоке
        //запускать в потоке нет нужды, скрипт работает быстро
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
                                sum = DoNoteRepeatYourSelf((difference.Days / 7), double.Parse(scheduleCells[4].ToString()));

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
                                sum = DoNoteRepeatYourSelf((difference.Days), double.Parse(scheduleCells[4].ToString()));

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
                                sum = DoNoteRepeatYourSelf((difference.Days), double.Parse(scheduleCells[4].ToString()));

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
            FillTransactionCards();
            ShowCards();
        }



        private void listOfOperationsGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void listOfOperationsGV_Selected(object sender, MouseButtonEventArgs e)
        {
            DataRowView card = (DataRowView)listOfOperationsGV.SelectedItem;

            if(card != null){

                var dr = card.Row[0];
                CARDID = Convert.ToInt32(dr);

                InfoPage = new InfoPage(CARDID);
                IncomeExpensesPage = new IncomeExpensesPage();
            }
        }
    }
}
