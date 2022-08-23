using DataAccess.Repositories;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        protected readonly IMemoryCache _memoryCache;
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
            Grades = new GradeRepo(_context);
            Students = new StudentRepo(_context);
            Subjects = new SubjectRepo(_context);
            Reports = new ReportRepo(_context);
            Exchanges = new ExchangeRepo(_memoryCache);
            //_memoryCache = memoryCache;
            //GradeSubjects = new GradeSubjectRepo(_context);
            // Loggers = new LoggerRepo(_context);
        }
        public IGradeRepo Grades { get; private set; }
        public IStudentRepo Students { get; private set; }
        public ISubjectRepo Subjects { get; private set; }
        public IReportRepo Reports { get; private set; }
        public IExchangeRepo Exchanges { get; private set; }
        //public IGradeSubjectRepo GradeSubjects { get; private set; }
        // public ILoggerRepo Loggers { get; private set; }

        public void AddLog(LogLevel logLevel, string message)
        {
            var log = new Log
            {
                LogLevel = logLevel.ToString(),
                Message = message,
            };
            _context.Add(log);
            _context.SaveChanges();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
