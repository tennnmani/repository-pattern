using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Student
    {
        public int StudentId { get;set; }
        public string FirstName { get;set; }
        public string LastName { get;set; }
        public int Age { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get;set; }
        public DateTime CreatedDate { get; set; }


        public int GradeId { get; set; }
        public Grade Grade { get; set; }
    }
}
