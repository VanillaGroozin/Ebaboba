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
    public class Currency : ICrud
    {
        public string Name { get; set; }
        public string CurrencyId { get; set; }

        public Currency() { }

        public Currency(string id)
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from Currency where @CurrencyId = CurrencyId");
            query.AddParameter("@CurrencyId", id.ToString());
            var dt = query.ExecuteAndGet();

            this.CurrencyId = id;
            this.Name = dt.Rows[0]["Name"].ToString();

            query.Clear();
        }

        public void Update()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("Name", this.Name);

            query.Add("update cr");
            query.AddUpdates(updates);
            query.Add("from Currency cr where CurrencyId = @CurrencyId");
            query.AddParameter("@CurrencyId", this.CurrencyId);
            query.Execute();
            query.Clear();
        }

        public void Insert()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);

            query.Add("insert into Currency");
            query.AddInsertValues(new List<string>() { this.Name });

            query.Execute();
            query.Clear();
        }

        public DataTable SelectAll()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"select * from Currency");
            var returnValue = query.ExecuteAndGet();
            query.Clear();
            return returnValue;
        }

        public void Delete()
        {
            QueryLite query = new QueryLite(ConfigurationManager.ConnectionStrings["EbabobaConnectionString"].ConnectionString);
            query.Add($"Delete from Currency where CurrencyId = @CurrencyId");
            query.AddParameter("@CurrencyId", this.CurrencyId);
            query.Execute();
            query.Clear();
        }
    }
}
