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
    public class SubjectRepo : GenericRepository<Subject>, ISubjectRepo
    {
        public SubjectRepo(DatabaseContext context) : base(context)
        {
        }

        public IQueryable<Subject> getFiltteredSubject(string searchs, string fromDate, string toDate)
        {
            var subjects = _context.Subjects
                         .Include(s => s.GradeSubject)
                             .ThenInclude(i => i.Grade).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                subjects = subjects.Where(s => s.SubjectName.Contains(searchs));
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                subjects = subjects.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return subjects;
        }

        public void removeSubject(Subject s)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var gS = _context.GradeSubjects.Where(g => g.SubjectId == s.SubjectId);
                    _context.GradeSubjects.RemoveRange(gS);

                    _context.Subjects.Remove(s);

                    _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)

                }
            }
        }
    }
}
