using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReportRepo
    {
        List<MasterVM> GetJoinedDateCount();
        List<MasterVM> StudentSubjectCount();
        List<AgeVM> GradeAge();
        List<MasterVM> GradeAgeSum();
        List<MasterVM> GradeAgeMin();
        List<MasterVM> GradeAgeMax();
        List<MasterVM> GradeAgeAvrage();
    }
}
