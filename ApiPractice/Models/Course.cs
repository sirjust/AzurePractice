namespace ApiPractice.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }

        public Course(int id, string name)
        {
            CourseID = id;
            CourseName = name;
        }
    }
}
