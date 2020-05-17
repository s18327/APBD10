using APBD10.Entities;

namespace APBD10.DTO
{
    public class PromoteResponse
    {
        private Enrollment enrollment;

        public string Studies { get; set; }

        public int Semester { get; set; }

        public PromoteResponse(Enrollment enrollment)
        {
            Studies = enrollment.StartDate.ToString();

            Semester = enrollment.Semester;
        }
    }
}