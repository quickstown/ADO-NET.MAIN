namespace College_DB.Models
{ 

    public class Grades
    {
        public int GradesId { get; set; } = 0;

        public int StudentId { get; set; }

        public Student? Student { get; set; }

        public int SubjectsId { get; set; }

        public Subjects? Subjects { get; set; }

        public int Score { get; set; }

        public DateTime GradeDate { get; set; }

    }
}