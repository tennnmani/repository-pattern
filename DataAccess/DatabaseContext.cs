using Microsoft.EntityFrameworkCore;
//using MVC2.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<GradeSubject> GradeSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student").HasKey(t => t.StudentId);
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsRequired().HasMaxLength(225);
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsRequired().HasMaxLength(225);
            modelBuilder.Entity<Student>().Property(p => p.Age).IsRequired();
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Grade)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GradeId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Student>().Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Grade>().ToTable("Grade").HasKey(g => g.GradeId);
            modelBuilder.Entity<Grade>().Property(p => p.GradeName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Grade>().Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Subject>().ToTable("Subject").HasKey(g => g.SubjectId);
            modelBuilder.Entity<Subject>().Property(p => p.SubjectName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Subject>().Property<DateTime>("CreatedDate");

            modelBuilder.Entity<GradeSubject>()
                .HasKey(c => new { c.GradeId, c.SubjectId });
            modelBuilder.Entity<GradeSubject>().Property<DateTime>("CreatedDate");

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added);

            foreach (var entityEntry in entries)
            {
                    entityEntry.Property("CreatedDate").CurrentValue = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added);

            foreach (var entityEntry in entries)
            {
                    entityEntry.Property("CreatedDate").CurrentValue = DateTime.Now;
            }

            return base.SaveChanges();
        }
    }
}
