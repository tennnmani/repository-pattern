using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace repository_pattern.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentController> _logger;
        private readonly IMemoryCache _memoryCache;
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        public StudentController(IUnitOfWork unitOfWork, ILogger<StudentController> logger, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> Index(
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

            var students = _unitOfWork.Students.getFiltteredStudent(searchString, fromDate, toDate);

            int ps = _unitOfWork.Students.PageSize(pageSize);

            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(students.Count() / (double)ps),
                PageIndex = pageNum,
            };
            var display = await students.Skip((pageNum - 1) * ps).Take(ps).ToListAsync();
            return View(display);

            //return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, ps));
        }


        public async Task<IActionResult> Details(int id, string name)
        {
            var student = await _unitOfWork.Students.getStudentWGradeNSub(id, name);

            return View(student);
        }
        public IActionResult Create()
        {
            ViewData["GradeList"] = new SelectList(GetGrades(), "GradeId", "GradeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstName", "LastName", "DOB", "GradeId")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                 //   _logger.LogInformation($"Student {student.FirstName} Saved");
                   // throw new NullReferenceException();
                    _unitOfWork.Students.Add(student);
                    _unitOfWork.Complete();
                    _logger.LogInformation($"Student \"{student.FirstName}\" Saved");
                }
                catch(Exception ex)
                {
                    //add log to db
                    _logger.LogError($"Error at saving student msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at saving student msg: {ex.Message}");
                }
                return RedirectToAction("Index");
            }

            ViewData["GradeList"] = new SelectList(GetGrades(), "GradeId", "GradeName");
            return View(student);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = _unitOfWork.Students.GetById(id ?? 0);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["GradeList"] = new SelectList(GetGrades(), "GradeId", "GradeName");
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("StudentId", "FirstName", "LastName", "DOB", "GradeId")] Student student)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Students.Update(student);
                    _unitOfWork.Complete();

                    _logger.LogInformation($"Student \"{student.FirstName}\" Editted");
                }
                catch(Exception ex)
                {
                    //add log to db
                    _logger.LogError($"Error at Editting student msg: {ex.Message}");
                    _unitOfWork.AddLog(LogLevel.Error, $"Error at Editting student msg: {ex.Message}");
                }
                return RedirectToAction("Index");

            }

            ViewData["GradeList"] = new SelectList(GetGrades(), "GradeId", "GradeName");
            return View(student);
        }

        public IActionResult Delete(int id)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _unitOfWork.Students.Remove(student);
                _unitOfWork.Complete();
                _logger.LogInformation($"Student \"{student.FirstName}\" Deleted");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error at Deleting student msg: {ex.Message}");
                _unitOfWork.AddLog(LogLevel.Error, $"Error at Deleting student msg: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private List<Grade> GetGrades()
        {
            List<Grade> grades = new List<Grade>();

            IDatabase db = redis.GetDatabase();

            var gradeinit = db.StringGet("GradeList");
            if (gradeinit.HasValue)
            {
                return JsonConvert.DeserializeObject<List<Grade>>(gradeinit);
            }
            else
            {
                grades = _unitOfWork.Grades.GetAll().Select(s=> new Grade {GradeId = s.GradeId , GradeName = s.GradeName}).ToList();
                db.StringSet("GradeList", JsonConvert.SerializeObject(grades), TimeSpan.FromMinutes(10));
                return grades;
            }

            //if (!_memoryCache.TryGetValue("GradeList", out grades))
            //{
            //    grades = _unitOfWork.Grades.GetAll().ToList();
            //    _memoryCache.Set<List<Grade>>("GradeList", grades, TimeSpan.FromDays(10));
            //}
            //return grades;
        }
    }
}
