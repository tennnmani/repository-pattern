using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace repository_pattern.Controllers
{
    public class ExchangeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExchangeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
