using Ebabobo.Models;
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

namespace Ebabobo.Pages
{
    /// <summary>
    /// Логика взаимодействия для OperationsPage.xaml
    /// </summary>
    public partial class OperationsPage : Page
    {
        public OperationsPage()
        {
            InitializeComponent();
            FillCardCb();
            FillCurrencyCb();
            FillCategoryCb();
            FillOperationsList();
        }

     

        private void FillOperationsList()
        {
            var dt = new History().GetTransactionsInfo(
                outcome: (bool)chb_OnlyOutcome.IsChecked,
                income: (bool)chb_OnlyIncome.IsChecked,
                dateBegin: First_date.SelectedDate,
                dateEnd: Second_date.SelectedDate,
                type: cbTypeName.SelectedValue,
                currency: cbCurrencyName.SelectedValue,
                card: cbCard.SelectedValue);

            listOfOperationsGV.DataContext = dt.DefaultView;
        }
        private void FillCurrencyCb()
        {
            var dt = new Currency().SelectAll();
            cbCurrencyName.ItemsSource = dt.DefaultView;
            cbCurrencyName.DisplayMemberPath = "Name";
            cbCurrencyName.SelectedValuePath = "CurrencyId";
        }

        private void FillCategoryCb()
        {
            var dt = new TransactionType().SelectAll();
            cbTypeName.ItemsSource = dt.DefaultView;
            cbTypeName.DisplayMemberPath = "Name";
            cbTypeName.SelectedValuePath = "TransactionTypeId";
        }

        private void FillCardCb()
        {
            var dt = new Card().SelectAll();
            cbCard.ItemsSource = dt.DefaultView;
            cbCard.DisplayMemberPath = "Name";
            cbCard.SelectedValuePath = "CardId";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FillOperationsList();
        }
    }
}
