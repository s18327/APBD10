using APBD10.DTO;
using APBD10.Entities;

namespace APBD10.Services
{
    public interface IStudentServiceDb
    {
        EnrollmentResponse EnrollStudent(EnrollmentRequest req);

        PromoteResponse PromoteStudent(int semester, string studies);

        Student GetStudent(string indexNumber);

        public void SaveLogData(string data);
    }
}