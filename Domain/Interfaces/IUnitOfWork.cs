﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISubjectRepo Subjects { get; }
        IStudentRepo Students { get; }
        IGradeRepo Grades { get; }
        IReportRepo Reports { get; }
        IExchangeRepo Exchanges { get; }
        //IGradeSubjectRepo GradeSubjects { get; }
        int Complete();
    }
}
