namespace College_DB.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public List<Subjects> Subjects { get; set; } = new();

    }
}