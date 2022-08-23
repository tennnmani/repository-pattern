using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ExchangeRepo : IExchangeRepo
    {

        public async Task<List<Exchange>> CallExchange()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<Exchange> exchange = new List<Exchange>();
            string apiUrl = "https://www.nrb.org.np/api/forex/v1/app-rate";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            //_memoryCache.Set<List<Exchange>>(getCacheKey(DateTime.Now.AddDays(-1).Date.ToString()), exchange, new DateTimeOffset().AddDays(1));

            exchange = JsonConvert.DeserializeObject<List<Exchange>>(await response.Content.ReadAsStringAsync());


            return exchange;
        }

        public async Task<Decimal> GetRate(string fromCurrency, string toCurrency, string buySell)
        {
            List<Exchange> exchange = await CallExchange();

            decimal fromPrice = 0;
            decimal toPrice = 0;


            if (fromCurrency == "NEP")
            {
                fromPrice = 1;
            }
            else
            {
                var data = (from e in exchange
                            where e.iso3 == fromCurrency
                            select new { e.buy, e.sell, e.unit }).FirstOrDefault();

                if (buySell == "B")
                    fromPrice = data.buy;
                else
                    fromPrice = data.sell;

                decimal unit = data.unit;
                if (unit > 1)
                    fromPrice /= unit;

            }

            if (toCurrency == "NEP")
            {
                toPrice = 1;
            }
            else
            {
                var data = (from e in exchange
                            where e.iso3 == toCurrency
                            select new { e.buy, e.sell, e.unit }).FirstOrDefault();

                if (buySell == "B")
                    toPrice = data.buy;
                else
                    toPrice = data.sell;

                decimal unit = data.unit;
                if (unit > 1)
                    toPrice /= unit;

            }

            //foreach (var i in exchange)
            //{
            //    if (i.iso3 == fromCurrency)
            //    {
            //        if (buySell == "B")
            //            fromPrice = Decimal.Parse(i.buy);
            //        else
            //            fromPrice = Decimal.Parse(i.sell);
            //        fromRat = i.unit;

            //        if (fromRat > 1)
            //        {
            //            fromPrice = fromPrice / fromRat;
            //        }
            //    }
            //    if (i.iso3 == toCurrency)
            //    {
            //        if (buySell != "B")
            //            toPrice = Decimal.Parse(i.buy);
            //        else
            //            toPrice = Decimal.Parse(i.sell);
            //        toRat = i.unit;

            //        if (toRat > 1)
            //        {
            //            toPrice = toPrice / toRat;
            //        }
            //    }
            //}

            return (fromPrice / toPrice);
        }

        private string getCacheKey(string date)
        {
            return $"exchangefor{date}";
        }
    }
}
