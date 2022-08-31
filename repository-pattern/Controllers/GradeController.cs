using DataAccess;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace repository_pattern.Controllers
{
    public class GradeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GradeController> _logger;
        private readonly IMemoryCache _memoryCache;
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");

        public GradeController(IUnitOfWork unitOfWork, ILogger<GradeController> logger, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public IActionResult Index(
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
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var grade = _unitOfWork.Grades.GetFiltteredGrade(searchString, fromDate, toDate);

            int ps = _unitOfWork.Grades.PageSize(pageSize);

            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(grade.Count() / (double)ps),
                PageIndex = pageNum,
            };

            var display = grade.Skip((pageNum - 1) * ps).Take(ps).ToList();
            return View(display);

            //return View(await PaginatedList<Grade>.CreateAsync(grade, pageNumber ?? 1, ps));
        }

        public IActionResult Create()
        {
            ViewData["Subject"] = GetSubjectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("GradeName")] Grade grade, string[] subjectIndex)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Grades.CreateGrade(grade, subjectIndex);

                    //delete cache
                    IDatabase db = redis.GetDatabase();
                    db.KeyDelete("GradeList");
                   // _memoryCache.Remove("GradeList");

                    _logger.LogInformation($"Grade \"{grade.GradeName}\" Saved");
                }
                catch(Exception ex)
                {
                    //add log to db
                    _logger.LogError($"Error at saving Grade msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at saving Grade msg: {ex.Message}");
                }
                return RedirectToAction("Index");

            }
            ViewData["Subject"] = GetSubjectList();
            return View(grade);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _unitOfWork.Grades.GetGradeWSubject(id);
            if (grade == null)
            {
                return NotFound();
            }

            ViewData["Subject"] = GetSubjectList();
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("GradeId", "GradeName")] Grade grade, string[] subjectIndex)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //throw new Exception("ERROR");
                    _unitOfWork.Grades.UpdateGrade(grade, subjectIndex);
                    
                    IDatabase db = redis.GetDatabase();
                    db.KeyDelete("GradeList");
                    //_memoryCache.Remove("GradeList");


                    _logger.LogInformation($"Grade \"{grade.GradeName}\" Editted");
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error at Editting Grade msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at Editting Grade msg: {ex.Message}");
                }
                return RedirectToAction("Index");
            }

            ViewData["Subject"] = GetSubjectList();
            return View(grade);
        }

        public IActionResult Delete(int id)
        {
            var grade = _unitOfWork.Grades.GetById(id);
            if (grade == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _unitOfWork.Grades.RemoveGrade(grade);

                IDatabase db = redis.GetDatabase();
                db.KeyDelete("GradeList");
                //_memoryCache.Remove("GradeList");
                
                _logger.LogInformation($"Grade \"{grade.GradeName}\" Deleted");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error at Deleting Grade msg: {ex.Message}");
                _unitOfWork.AddLog(LogLevel.Error, $"Error at Deleting Grade msg: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }

        private List<Subject> GetSubjectList()
        {
            List<Subject> subjects = new List<Subject>();

            IDatabase db = redis.GetDatabase();
            //var grades = _unitOfWork.Grades.GetAll().ToList();

            var subjectlistint = db.StringGet("SubjectList");
            if (subjectlistint.HasValue)
            {
                return JsonConvert.DeserializeObject<List<Subject>>(subjectlistint);
            }
            else
            {
                subjects = _unitOfWork.Subjects.GetAll().Select( s=> new Subject { SubjectId = s.SubjectId, SubjectName = s.SubjectName }).ToList();
                db.StringSet("SubjectList", JsonConvert.SerializeObject(subjects), TimeSpan.FromMinutes(10));
                return subjects;
            }

            //if (!_memoryCache.TryGetValue("SubjectList", out subjects))
            //{
            //    subjects = _unitOfWork.Subjects.GetAll().ToList();
            //    _memoryCache.Set<List<Subject>>("SubjectList", subjects, TimeSpan.FromDays(10));
            //}
            //return subjects;
        }
    }
}
