using APBD10.DTO;
using APBD10.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD10.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentServiceDb service;

        public EnrollmentsController(IStudentServiceDb service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }

            var enroll = service.EnrollStudent(request);

            if (enroll != null)
            {
                return Ok(enroll);
            }
            else
            {
                return BadRequest("bad request");
            }
        }

        [HttpPost("promote")]
        public IActionResult PromoteStudents(PromoteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }

            var result = service.PromoteStudent(request.Semester, request.Studies);

            if (result != null)
            {
                return Ok(result);
            }
            else

            {
                return BadRequest("bad request");
            }
        }
    }
}