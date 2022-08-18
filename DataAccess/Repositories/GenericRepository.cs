using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DatabaseContext _context;
        public GenericRepository(DatabaseContext context)
        {
            _context = context;
        }

        //public void AddLog(LogLevel logLevel, string message)
        //{
        //    var log = new Log
        //    {
        //        LogLevel = logLevel.ToString(),
        //        Message = message,
        //    };
        //    _context.Add(log);
        //    _context.SaveChanges();
        //}
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        public int PageSize(string pageSize)
        {
            // get page sie from app.jsomn
            int ps = 10;

            if (pageSize != null)
            {
                ps = Int32.Parse(pageSize);
            }
            if (ps == 0)
            {
                ps = Count();
            }

            return ps;
        }
        public int Count()
        {
            return _context.Set<T>().Count();
        }
    }
}
