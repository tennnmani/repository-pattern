﻿using DataAccess;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace repository_pattern.Controllers
{
    public class GradeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public GradeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            ViewData["Subject"] = _unitOfWork.Subjects.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("GradeName")] Grade grade, string[] subjectIndex)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Grades.CreateGrade(grade, subjectIndex);

                return RedirectToAction("Index");

            }
            ViewData["Subject"] = _unitOfWork.Subjects.GetAll();
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

            ViewData["Subject"] = _unitOfWork.Subjects.GetAll();
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("GradeId", "GradeName")] Grade grade, string[] subjectIndex)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Grades.UpdateGrade(grade, subjectIndex);
                return RedirectToAction("Index");
            }

            ViewData["Subject"] = _unitOfWork.Subjects.GetAll();
            return View(grade);
        }

        public IActionResult Delete(int id)
        {
            var grade = _unitOfWork.Grades.GetById(id);
            if (grade == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Grades.RemoveGrade(grade);
            return RedirectToAction(nameof(Index));
        }
    }
}
