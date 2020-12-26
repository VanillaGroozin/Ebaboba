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
    public class Card : ICrud
    {
        public string CardId { get; set; }
        public string Name { get; set; }
        public string Sum { get; set; }
        public string CurrencyId { get; set; }
        public Card() { }
        public Card(string id)
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from Card where CardId = @CardId");
            query.AddParameter("@CardId", id.ToString());
            var dt = query.ExecuteAndGet();

            this.CardId = id;
            this.Sum = dt.Rows[0]["Sum"].ToString();
            this.Name = dt.Rows[0]["Name"].ToString();
            this.CurrencyId = dt.Rows[0]["CurrencyId"].ToString();

            query.Clear();
        }

        public void Update()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("Name", this.Name);
            updates.Add("Sum", this.Sum.Replace(',','.'));
            updates.Add("CurrencyId", this.CurrencyId);

            query.Add("update crd");
            query.AddUpdates(updates);
            query.Add("from Card crd where CardId = @CardId");
            query.AddParameter("@CardId", this.CardId);
            query.Execute();
            query.Clear();
        }

        public void Insert()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            query.Add("insert into Card");
            query.AddInsertValues(new List<string>() { this.Name, this.CardId, this.CurrencyId });
            query.Execute();
            query.Clear();
        }

        public DataTable SelectAll()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from Card");
            var returnValue = query.ExecuteAndGet();
            query.Clear();
            return returnValue;
        }

        public DataTable SelectAllByCurrency(string currencyId)
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from Card where @CurrencyId = CurrencyId");
            query.AddParameter("@CurrencyId", currencyId);
            var returnValue = query.ExecuteAndGet();
            query.Clear();
            return returnValue;
        }

        public int SelectCardsCount()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select Count(CardId) from Card");
            var returnValue = query.ExecuteAndGet();
            query.Clear();

            int count = returnValue.Rows.Count;
            return count;
        }

        public void Delete()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"Delete from Card where CardId = @CardId");
            query.AddParameter("@CardId", this.CardId);
            query.Execute();
            query.Clear();
        }

        DataTable ICrud.SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}
