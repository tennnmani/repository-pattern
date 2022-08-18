//using Domain.Entities;
//using Domain.Interfaces;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace DataAccess.Repositories
{
//    public class LoggerRepo : ILoggerRepo
//    {
//        private readonly DatabaseContext _context;

//        public LoggerRepo(DatabaseContext context)
//        {
//            _context = context;
//        }

//        public void Log(LogLevel logLevel, string message)
//        {
//            if (logLevel == LogLevel.Error)
//            {
//                var log = new Log
//                {
//                    LogLevel = logLevel.ToString(),
//                    Message = message,
//                };
//                _context.Add(log);
//                _context.SaveChanges();
//            }
//            else
//            {
//                Console.WriteLine(message);
//            }

//        }
//    }
}
