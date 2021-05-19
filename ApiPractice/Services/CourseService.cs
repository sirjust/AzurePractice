using ApiPractice.Models;
using System.Collections.Generic;

namespace ApiPractice.Services
{
    public class CourseService
    {
        public List<Course> Courses { get; set; }

        public CourseService()
        {
            Courses = new List<Course>()
            {
                new Course(1, "AZ-204"),
                new Course(2, "AZ-303"),
                new Course(3, "AZ-304")
            };
        }

        public IEnumerable<Course> GetCourses()
        {
            return Courses;
        }
    }
}
