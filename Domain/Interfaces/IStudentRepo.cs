using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStudentRepo : IGenericRepository<Student>
    {
        IQueryable<Student> getFiltteredStudent(string searchs, string fromDate, string toDate);
        Task<Student> getStudentWGradeNSub(int id, string name);
    }
}
