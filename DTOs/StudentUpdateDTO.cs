using System.ComponentModel.DataAnnotations;

namespace EF_StoreProcedure.DTOs
{
    public class StudentUpdateDTO
    {
        [Required(ErrorMessage = "StudentId is required for updating a record.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        [MaxLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
        public string Branch { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [MaxLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = null!;
    }
}
