using APBD10.Entities;
using APBD10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APBD10.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext studentContext;

        public StudentsController(StudentContext studentContext)
        {
            this.studentContext = studentContext;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = studentContext.Student
                                            .Include(s => s.IdEnrollmentNav)
                                            .ThenInclude(s => s.IdStudyNav)
                                            .Select(st => new GetStudentResponse
                                            {
                                                IndexNumber = st.IndexNumber,
                                                FirstName = st.FirstName,
                                                LastName = st.LastName,
                                                BirthDate = st.BirthDate.ToShortDateString(),
                                                Semester = st.IdEnrollmentNav.Semester,
                                                Studies = st.IdEnrollmentNav.IdStudyNav.Name,
                                            }).ToList();
            return Ok(students);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            // Check if data was delivered corectly
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }

            if (student != null)
            {
                studentContext.Add<Student>(student);
                studentContext.SaveChanges();

                return Ok(student);
            }
            else

            {
                return BadRequest("bad request");
            }
        }

        [HttpDelete("deleteStudent{id}")]
        public IActionResult GetStudents(string id)
        {
            var index = id;
            var student = (from c in studentContext.Student
                           where c.IndexNumber == id
                           select c).First();

            if (student != null)
            {
                studentContext.Student.Remove(student);
                studentContext.SaveChanges();
                return Ok("Succesfully deleted");
            }
            else
            {
                return BadRequest("Succesfully deleted");
            }
        }

        [HttpPut("updateStudent")]
        public IActionResult UpdateStudent(Student s)
        {
            var student = (from c in
                               studentContext.Student
                           where c.IndexNumber == s.IndexNumber
                           select c).First();

            if (student != null)
            {
                studentContext.Update<Student>(s);
                studentContext.SaveChanges();
                return Ok("Succesuflly Updated");
            }
            else
            {
                return BadRequest("Failed to Update");
            }
        }
    }
}