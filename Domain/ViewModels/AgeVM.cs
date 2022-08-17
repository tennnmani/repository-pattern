using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class AgeVM
    {
        public string Name { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal AverageAge { get; set; }
        public int SumAge { get; set; }
    }
}
