using System;
using System.ComponentModel.DataAnnotations;

namespace APBD10.DTO
{
    public class EnrollmentRequest
    {
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Studies { get; set; }
    }
}