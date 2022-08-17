using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExchangeRepo
    {
        Task<List<Exchange>> CallExchange();

        Task<Decimal> GetRate(string fromCurrency, string toCurrency, string buySell);
    }
}
