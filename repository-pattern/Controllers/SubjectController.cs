using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace repository_pattern.Controllers
{
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubjectController> _logger;
        private readonly IMemoryCache _memoryCache;
        public SubjectController(IUnitOfWork unitOfWork, ILogger<SubjectController> logger, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public  IActionResult Index(
           string currentFilter,
           string searchString,
           int? pageNumber,
           string fromDate,
           string toDate,
           string pageSize)
        {
            int pageNum = pageNumber ?? 1;
            if (searchString != null)
            {
                pageNum = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var subjects = _unitOfWork.Subjects.getFiltteredSubject(searchString, fromDate, toDate);

            int ps = _unitOfWork.Subjects.PageSize(pageSize);
           
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(subjects.Count() / (double)ps),
                PageIndex = pageNum,
            };


            //ViewData["TotalPage"] = (int)Math.Ceiling(subjects.Count() / (double)ps);
            //ViewData["PageIndex"] = pageNum;

            var display = subjects.Skip((pageNum - 1) * ps).Take(ps).ToList();
            return View(display);

            // return View(await PaginatedList<Subject>.CreateAsync(subjects, pageNumber ?? 1, ps));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create([Bind("SubjectName")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Subjects.Add(subject);
                    _unitOfWork.Complete();


                    _memoryCache.Remove("SubjectList");
                    _logger.LogInformation($"Subject \"{subject.SubjectName}\" Saved");
                }
                catch(Exception ex)
                {
                    //add log to db
                    _logger.LogError($"Error at saving Subject msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at saving Subject msg: {ex.Message}");
                }

                return RedirectToAction("Index");

            }
            return View(subject);
        }

        public  IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = _unitOfWork.Subjects.GetById(id??0);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit([Bind("SubjectId", "SubjectName")] Subject subject)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //throw new Exception("ERROR");
                    _unitOfWork.Subjects.Update(subject);
                    _unitOfWork.Complete();

                    _memoryCache.Remove("SubjectList");
                    _logger.LogInformation($"Subject \"{subject.SubjectName}\" Editted");
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error at Editting Subject msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at Editting Subject msg: {ex.Message}");
                }

                return RedirectToAction("Index");
            }

            return View(subject);
        }

        public  IActionResult Delete(int id)
        {
            var subject = _unitOfWork.Subjects.GetById(id);
            if (subject == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _unitOfWork.Subjects.Remove(subject);
                _unitOfWork.Complete();

                _memoryCache.Remove("SubjectList");
                _logger.LogInformation($"Subject \"{subject.SubjectName}\" Deleted");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error at Deleting Subject msg: {ex.Message}");
                _unitOfWork.AddLog(LogLevel.Error, $"Error at Deleting Subject msg: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
