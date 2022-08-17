using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace repository_pattern.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            ViewData["Min"] = _unitOfWork.Reports.GradeAgeMin();
            ViewData["Max"] = _unitOfWork.Reports.GradeAgeMax();
            ViewData["Average"] = _unitOfWork.Reports.GradeAgeAvrage();
            ViewData["Sum"] = _unitOfWork.Reports.GradeAgeSum();

            return View(_unitOfWork.Reports.GradeAge());
        }

        public IActionResult JoinedCount()
        {
            return View(_unitOfWork.Reports.GetJoinedDateCount());
        }
    }
}
