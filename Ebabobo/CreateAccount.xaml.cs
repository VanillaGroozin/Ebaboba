using Ebabobo.Data;
using Ebabobo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Ebabobo
{
    /// <summary>
    /// Логика взаимодействия для CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {       
            InitializeComponent();
            FillCurrencyCb();
        }

        private void FillCurrencyCb()
        {
            var dt = new Currency().SelectAll();
            cbCurrency.ItemsSource = dt.DefaultView;
            cbCurrency.DisplayMemberPath = "Name";
            cbCurrency.SelectedValuePath = "CurrencyId";
        }

        private void addAccountBtn(object sender, RoutedEventArgs e)
        {
            DbManager dbManager = new DbManager();
            Card card = new Card();
            card.Name = tb_accountName.Text;
            card.CurrencyId = cbCurrency.SelectedValue.ToString();
            card.Sum = "0";
            dbManager.Insert(card);

            this.Close();
        }
    }
}
