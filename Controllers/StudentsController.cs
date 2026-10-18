using EF_StoreProcedure.Data;
using EF_StoreProcedure.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EF_StoreProcedure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly EF_StoreProcedureDbContext _context;

        public StudentsController(EF_StoreProcedureDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllStudent()
        {
            // Approach 1: Using FromSqlRaw (Static SQL - No Parameter)
            // Best suited for stored procedures without parameters.
            // Returns a result set mapped directly to the Students entity.
            var students = await _context.Students.FromSqlRaw("EXEC spGetAllStudents").ToListAsync();

            // Approach 2: Using FromSqlInterpolated (Parameterized & Safer)
            // Use this if the stored procedure accepts parameters, 
            // or if you want to maintain safer parameterization syntax.
            //var students = await _context.Students
            //    .FromSqlInterpolated($"EXEC spGetAllStudents")
            //    .ToListAsync();
            if(students == null || students.Count == 0)
            {
                return NotFound(new { Success = false, Message = "No student found."});
            }

            return Ok(new
            {
                Success = true,
                Message = "Student records fetched successfully.",
                Data = students
            });
        }

        // GET: Fetch Student By Id (Demonstrates FromSqlInterpolated & FromSqlRaw)
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetStudentById(int id)
        {

            // Approach 1: Using FromSqlInterpolated (Preferred)
            // Automatically parameterizes the query (prevents SQL injection).
            // Ideal when user input or dynamic values are involved.
            var students = await _context.Students.FromSqlInterpolated($"EXEC spGetStudentByStudentId {id}").ToListAsync();

            // Approach 2: Using FromSqlRaw (Manual Parameter)
            // Use @p0 style placeholders and pass parameters explicitly.
            // Equivalent to parameterized query, but syntax is less readable.
            //var students = await _context.Students
            //    .FromSqlRaw("EXEC spGetStudentByStudentId @p0", id)
            //    .ToListAsync();
            var student = students.FirstOrDefault();

            if (student == null)
                return NotFound(new
                {
                    Success = false,
                    Message = $"No student found with ID {id}."
                });

            return Ok(new
            {
                Success = true,
                Message = "Student record fetched successfully.",
                Data = student
            });
        }

        // POST: Insert a New Student 
        // Demonstrates ExecuteSqlInterpolated & ExecuteSqlRaw with OUTPUT parameter
        [HttpPost("Create")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Define OUTPUT parameter to capture the newly generated StudentId
            var studentIdParam = new SqlParameter
            {
                ParameterName = "@StudentId",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            // Approach 1: Using ExecuteSqlInterpolated (Preferred)
            // Cleaner syntax with string interpolation and automatic parameterization.
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC spInsertStudent {dto.FirstName}, {dto.LastName}, {dto.Branch}, {dto.Gender}, {dto.Email}, {dto.Phone}, {studentIdParam} OUTPUT");

            // Approach 2: Using ExecuteSqlRaw (Classic Style)
            // Use explicit positional parameters {0}, {1}, ... for SQL execution.
            //await _context.Database.ExecuteSqlRawAsync(
            //    "EXEC spInsertStudent @p0, @p1, @p2, @p3, @p4, @p5, @StudentId OUTPUT",
            //    dto.FirstName, dto.LastName!, dto.Branch, dto.Gender, dto.Email, dto.Phone, studentIdParam
            //);
            int newStudentId = (int)studentIdParam.Value;

            return Ok(new
            {
                Success = true,
                Message = "Student created successfully",
                StudentId = newStudentId
            });
        }

        // PUT: Update an Existing Student 
        // Demonstrates ExecuteSqlRaw & ExecuteSqlInterpolated
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentUpdateDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Approach 1: Using ExecuteSqlRaw (Positional Parameters)
            // Suitable for static queries; parameters are safely passed to SQL.
            int rowsAffected = await _context.Database.ExecuteSqlRawAsync("EXEC spUpdateStudent @p0, @p1, @p2, @p3, @p4, @p5, @p5, @p6", dto.StudentId, dto.FirstName, dto.LastName!, dto.Branch,
                dto.Gender, dto.Email, dto.Phone);

            // Approach 2: Using ExecuteSqlInterpolated (Cleaner Syntax)
            // Uses string interpolation with automatic parameterization.
            //int rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            //    $"EXEC spUpdateStudent {dto.StudentId}, {dto.FirstName}, {dto.LastName}, {dto.Branch}, {dto.Gender}, {dto.Email}, {dto.Phone}"
            //);

            if(rowsAffected == 0)
                return NotFound(new { Success = false, Message = "No student found to update." });

            return Ok(new
            {
                Success = true,
                Message = "Student updated successfully.",
                AffectedRows = rowsAffected
            });
        }

        // DELETE: Delete a Student 
        // Demonstrates ExecuteSqlRaw & ExecuteSqlInterpolated
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            // Approach 1: Using ExecuteSqlRaw (Traditional)
            // Uses positional parameter placeholders like @p0.
            int rowsAffected = await _context.Database.ExecuteSqlRawAsync("EXEC spDeleteStudent @p0", id);

            // Approach 2: Using ExecuteSqlInterpolated (Simpler Syntax)
            // Safer and more readable when parameters come from user input.
            //int rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            //    $"EXEC spDeleteStudent {id}"
            //);

            if (rowsAffected == 0)
                return NotFound(new { Success = false, Message = $"No student found with ID {id}." });

            return Ok(new
            {
                Success = true,
                Message = "Student deleted successfully.",
                AffectedRows = rowsAffected
            });
        }
    }
}
