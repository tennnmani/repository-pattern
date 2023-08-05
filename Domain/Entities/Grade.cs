using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Grade
    {
        public int GradeId { get; set; }
        [Display(Name ="Grade")]
        public string GradeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedDate1 { get; set; }
        public DateTime CreatedDate2 { get; set; }
        public DateTime CreatedDate3 { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<GradeSubject> SubjectsTaught { get; set; }
    }
}
