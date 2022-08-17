using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class GradeRepo : GenericRepository<Grade>, IGradeRepo
    {
       // private readonly IUnitOfWork _unitOfWork;
        public GradeRepo(DatabaseContext context) : base(context)
        {
         //   _unitOfWork = unitOfWork;
        }

        public void CreateGrade(Grade grade, string[] subjectIndex)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //_unitOfWork.Grades.Add(grade);
                    //_unitOfWork.Complete();
                    _context.Add(grade);
                    _context.SaveChanges();

                    foreach (var i in subjectIndex)
                    {
                        var gradeSubject = new GradeSubject();
                        gradeSubject.SubjectId = Int32.Parse(i);
                        gradeSubject.GradeId = grade.GradeId;
                        _context.Add(gradeSubject);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public IQueryable<Grade> GetFiltteredGrade(string searchs, string fromDate, string toDate)
        {
            var grade = _context.Grades.Include(g => g.SubjectsTaught)
                            .ThenInclude(s => s.Subject).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                grade = grade.Where(s => s.GradeName.Contains(searchs));
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                grade = grade.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return grade;
        }

        public Task<Grade> GetGradeWSubject(int? id)
        {
            return _context.Grades.Include(s => s.SubjectsTaught).FirstAsync(g => g.GradeId == id);
        }

        public void RemoveGrade(Grade grade)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var gradeSubject = _context.GradeSubjects.Where(g => g.GradeId == grade.GradeId);
                    _context.RemoveRange(gradeSubject);

                    _context.Grades.Remove(grade);

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {

                }

            }
        }

        public void UpdateGrade(Grade grade, string[] subjectIndex)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Update(grade);

                    //delete grade subject
                    var gS = _context.GradeSubjects.Where(g => g.GradeId == grade.GradeId);
                    _context.RemoveRange(gS);

                    foreach (var i in subjectIndex)
                    {
                        var sG = new GradeSubject();
                        sG.SubjectId = Int32.Parse(i);
                        sG.GradeId = grade.GradeId;
                        _context.Add(sG);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                }
                catch (DbUpdateException ex)
                {

                }
            }
        }
    }
}
