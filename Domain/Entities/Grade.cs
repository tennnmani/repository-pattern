using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Grade
    {
        public int GradeId { get; set; }
        [Display(Name ="Grade")]
        public string GradeName { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<GradeSubject> SubjectsTaught { get; set; }
    }
}
