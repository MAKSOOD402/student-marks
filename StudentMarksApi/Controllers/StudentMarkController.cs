using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMarksApi.Data;
using StudentMarksApi.Models;

namespace StudentMarksApi.Controllers;

[ApiController]
[Route("api/studentMark")]
public class StudentMarkController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudentMarkController> _logger;

    public StudentMarkController(AppDbContext context, ILogger<StudentMarkController> logger)
    {
        _context = context;
        _logger = logger;
    }


    // GET: api/student-marks/1
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentMark>> GetById(int id)
    {
        try
        {
            _logger.LogInformation($"GetById invoked with id: {id}");

            var studentMark = await _context.StudentMarks
        .FromSqlRaw(
            $"SELECT * FROM dbo.get_student_marks_by_id({id})")
        .AsNoTracking()
        .FirstOrDefaultAsync();

            //var student = await _context.StudentMarks.FindAsync(id);

            if (studentMark == null)
            {
                _logger.LogWarning($"Student record not found for id: {id}");
                return NotFound(new { message = "Student record not found." });
            }

            _logger.LogInformation($"Student record retrieved successfully for id: {id}");
            return Ok(studentMark);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetById for id: {id}");
            throw;
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentMark>>> GetAll()
    {
        try
        {
            _logger.LogInformation("GetAll invoked");

            var studentMarks = await _context.StudentMarks
                .FromSqlRaw("SELECT * FROM dbo.StudentMarks")
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation($"Query completed successfully. Total records: {studentMarks.Count}");
            return Ok(studentMarks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAll");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> Insert(StudentMark studentMark)
    {
        try
        {
            _logger.LogInformation($"Insert invoked for student: {studentMark.name}, subject: {studentMark.subject}, marks: {studentMark.marks}");

            await _context.Database.ExecuteSqlInterpolatedAsync($"""
            EXEC dbo.insert_student_marks
                @p_name={studentMark.name},
                @p_subject={studentMark.subject},
                @p_marks={studentMark.marks}
            """);

            _logger.LogInformation($"Student mark inserted successfully for: {studentMark.name}");
            return Ok("Student mark inserted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Insert for student: {studentMark.name}");
            throw;
        }
    }

    [HttpPut("{id}/marks")]
    public async Task<IActionResult> UpdateMarks(int id, int marks)
    {
        try
        {
            _logger.LogInformation($"UpdateMarks invoked for id: {id}, new marks: {marks}");

            await _context.Database.ExecuteSqlInterpolatedAsync($"""
            EXEC dbo.update_student_marks
                @p_id={id},
                @p_marks={marks}
            """);

            _logger.LogInformation($"Student mark updated successfully for id: {id}");
            return Ok("Student mark updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in UpdateMarks for id: {id}");
            throw;
        }
    }


    // DELETE: api/student-marks/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation($"Delete invoked for id: {id}");

            await _context.Database.ExecuteSqlInterpolatedAsync($"""
            EXEC dbo.delete_student_marks
               @p_id={id}
            """);

            _logger.LogInformation($"Student record deleted successfully for id: {id}");
            return Ok(new { message = "Student record deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Delete for id: {id}");
            throw;
        }
    }
}