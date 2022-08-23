using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace repository_pattern.Controllers
{
    public class ExchangeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public ExchangeController(IUnitOfWork unitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.Exchanges.CallExchange());
        }


        public async Task<JsonResult> GetRates(string fromCurrency, string toCurrency, string buySell)
        {
            return Json(await _unitOfWork.Exchanges.GetRate(fromCurrency, toCurrency, buySell));
        }
    }
}
