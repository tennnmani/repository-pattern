using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISubjectRepo : IGenericRepository<Subject>
    {
        IQueryable<Subject> getFiltteredSubject(string searchs, string fromDate, string toDate);
        void removeSubject(Subject s);
    }
}
