using Ebabobo.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebabobo.Models
{
    public class History : ICrud
    {
        public string HistoryId { get; set; }
        public string CardId { get; set; }
        public string Date { get; set; }
        public string IsIncome { get; set; }
        public string Sum { get; set; }
        public string TypeId { get; set; }
        public string CurrencyId { get; set; }

        public History() { }
        public History(string id)
        {
            QueryLite query = new QueryLite("*Connection string here*");
            query.Add($"select * from History where @id = {id}");
            query.AddParameter("id", id.ToString());
            var dt = query.ExecuteAndGet();

            this.HistoryId = id;
            this.CardId = dt.Rows[0]["CardId"].ToString();
            this.Date = dt.Rows[0]["Date"].ToString();
            this.IsIncome = dt.Rows[0]["IsIncome"].ToString();
            this.Sum = dt.Rows[0]["Sum"].ToString();
            this.TypeId = dt.Rows[0]["TypeId"].ToString();
            this.CurrencyId = dt.Rows[0]["CurrencyId"].ToString();

            query.Clear();
        }

        public void Update()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("CardId", this.CardId);
            updates.Add("Date", this.Date);
            updates.Add("IsIncome", this.IsIncome);
            updates.Add("Sum", this.Sum);
            updates.Add("TypeId", this.TypeId);
            updates.Add("CurrencyId", this.CurrencyId);

            query.Add("update hs");
            query.AddUpdates(updates);
            query.Add("from History hs where HistoryId = @HistoryId");
            query.AddParameter("@HistoryId", this.HistoryId);
            query.Execute();
            query.Clear();
        }


        public void Insert()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            query.Add("insert into History");
            query.AddInsertValues(new List<string>() { this.CardId, this.Date, this.IsIncome, this.Sum, this.TypeId, this.CurrencyId });

            query.Execute();
            query.Clear();
        }

        public DataTable SelectAll()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from History");
            var returnValue = query.ExecuteAndGet();
            query.Clear();
            return returnValue;
        }

        public Dictionary<string, double> GetHistoryByCard(string cardId, bool isIncome)
        {
            Dictionary<string, double> cardHistory = new Dictionary<string, double>();

            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select Sum(Sum) as Sum, Date from History where CardId = @CardId and IsIncome = @IsIncome group by Date order by Date");
            query.AddParameter("@CardId", cardId);
            query.AddParameter("@isIncome", isIncome ? "1" : "0");
            var dt = query.ExecuteAndGet();

            foreach (DataRow row in dt.Rows)
            {
                cardHistory.Add(Convert.ToDateTime(row["Date"].ToString()).ToShortDateString(), double.Parse(row["Sum"].ToString()));
            }

            query.Clear();
            return cardHistory;

        }

        public DataTable GetTransactionsInfo(bool outcome, bool income, DateTime? dateBegin, DateTime? dateEnd, object type, object currency, object card)
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select tt.Name 'Тип', concat((case h.IsIncome when 1 then '+' else '-' end), h.Sum) 'Cумма', c.Name 'Валюта', h.Date 'Дата' from History h " +
                $"join Currency c on h.CurrencyId = c.CurrencyId " +
                $"left join TransactionType tt on tt.TransactionTypeId = h.TypeId ");
            var wheres = new Dictionary<string, string>();
            
            if (type != null) wheres.Add("h.TypeId", type.ToString());
            if (currency != null) wheres.Add("h.Currencyid", currency.ToString());
            if (card != null) wheres.Add("h.CardId", card.ToString());
            if (dateEnd != null) query.Add($" and '{dateEnd.ToString()}' > h.Date");
            if (dateBegin != null) query.Add($" and '{dateBegin.ToString()}' < h.Date");
            if (outcome && income) query.Add(" and h.IsIncome in (1, 0)");
            else if (!outcome && income) query.Add(" and h.IsIncome = 1");
            else if (outcome && !income) query.Add(" and h.IsIncome = 0");

            query.AddWheres(wheres);
            var returnValue = query.ExecuteAndGet();
            query.Clear();
            return returnValue;
        }

        public void Delete()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"Delete from History where HistoryId = @HistoryId");
            query.AddParameter("@HistoryId", this.HistoryId);
            query.Execute();
            query.Clear();
        }
    }
}
