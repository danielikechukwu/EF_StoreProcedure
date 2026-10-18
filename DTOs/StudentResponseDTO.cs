namespace EF_StoreProcedure.DTOs
{
    public class StudentResponseDTO
    {
        public int StudentId { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Branch { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
    }
}
