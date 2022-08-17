using DataAccess.Repositories;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
            Grades = new GradeRepo(_context);
            Students = new StudentRepo(_context);
            Subjects = new SubjectRepo(_context);
            Reports = new ReportRepo(_context);
            Exchanges = new ExchangeRepo(_context);
            //GradeSubjects = new GradeSubjectRepo(_context);
        }
        public IGradeRepo Grades { get; private set; }
        public IStudentRepo Students { get; private set; }
        public ISubjectRepo Subjects { get; private set; }
        public IReportRepo Reports { get; private set; }
        public IExchangeRepo Exchanges { get; private set; }
        //public IGradeSubjectRepo GradeSubjects { get; private set; }


        public int Complete()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch(Exception ex)
            {

                return 0;
            }
           
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
