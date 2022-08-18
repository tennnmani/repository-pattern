using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace repository_pattern.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentController> _logger;
        public StudentController(IUnitOfWork unitOfWork, ILogger<StudentController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            ViewData["GradeList"] = new SelectList(_unitOfWork.Grades.GetAll(), "GradeId", "GradeName");
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
            ViewData["GradeList"] = new SelectList(_unitOfWork.Grades.GetAll(), "GradeId", "GradeName");
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

            ViewData["GradeList"] = new SelectList(_unitOfWork.Grades.GetAll(), "GradeId", "GradeName");
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

            ViewData["GradeList"] = new SelectList(_unitOfWork.Grades.GetAll(), "GradeId", "GradeName");
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
    }
}
