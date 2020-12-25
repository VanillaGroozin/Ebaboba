using Ebabobo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
    /// Логика взаимодействия для InfoPage.xaml
    /// </summary>
    public partial class InfoPage : Page
    {
        public InfoPage()
        {
            InitializeComponent();

            Card card = new Card("1");
            CardName.Text = card.Name;
            DrawIncomeChart(1);
            DrawOutcomeChart(1);
        }

        public InfoPage(int id)
        {
            InitializeComponent();

            Card card = new Card(id.ToString());
            CardName.Text = card.Name;
            DrawIncomeChart(id);
            DrawOutcomeChart(id);
        }

        private void DrawIncomeChart(int cardId)
        {
            var allHistory = new History().GetHistoryByCard(cardId.ToString(), true);
            if (allHistory.Count != 0)
            {
                ((ColumnSeries)ChartIncome.Series[0]).ItemsSource = allHistory;
            }
        }
        private void DrawOutcomeChart(int cardId)
        {
            var allHistory = new History().GetHistoryByCard(cardId.ToString(), false);
            if (allHistory.Count != 0)
            {
                ((ColumnSeries)ChartOutcome.Series[0]).ItemsSource = allHistory;
            }

        }
    }

}
