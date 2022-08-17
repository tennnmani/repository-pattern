using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGradeRepo : IGenericRepository<Grade>
    {
        IQueryable<Grade> GetFiltteredGrade(string searchs, string fromDate, string toDate);
        Task<Grade> GetGradeWSubject(int? id);
        void UpdateGrade(Grade graded, string[] subjectIndex);
        void CreateGrade(Grade grade, string[] subjectIndex);
        void RemoveGrade(Grade grade);
    }
}
