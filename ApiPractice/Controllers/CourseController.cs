using ApiPractice.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiPractice.Controllers
{
    [ApiController]
    [Route("/api/Course")]
    public class CourseController : ControllerBase
    {
        private CourseService _courseService;

        public CourseController(CourseService service)
        {
            _courseService = service;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            return Ok(_courseService.GetCourses());
        }
    }
}
