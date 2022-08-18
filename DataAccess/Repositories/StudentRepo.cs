using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentRepo : GenericRepository<Student>, IStudentRepo
    {
        public StudentRepo(DatabaseContext context) : base(context)
        {
        }

        public IQueryable<Student> getFiltteredStudent(string searchs, string fromDate, string toDate)
        {
            var students = _context.Students.Include(g => g.Grade).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                students = students.Where(s => s.LastName.Contains(searchs)
                                       || s.FirstName.Contains(searchs)
                                       || s.Grade.GradeName.Contains(searchs)
                                        );
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                students = students.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return students;
        }

        public Task<Student> getStudentWGradeNSub(int id, string name)
        {
            return _context.Students.Include(g => g.Grade)
                        .ThenInclude(st => st.SubjectsTaught)
                            .ThenInclude(i => i.Subject)
                        .SingleAsync(s => s.StudentId == id);
        }
    }
}
