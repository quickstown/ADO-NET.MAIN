namespace College_DB.Models
{
    public class Subjects{

        public int SubjectsId { get; set; } = 0;

        public string SubjectsName { get; set; } = string.Empty;

        public int TeacherId  { get; set; }

        public Teacher? Teacher { get; set; }

        public List<Grades> Grades { get; set; } = new();

    }
}