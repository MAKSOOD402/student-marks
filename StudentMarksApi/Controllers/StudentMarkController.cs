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

    public StudentMarkController(AppDbContext context)
    {
        _context = context;
    }


    // GET: api/student-marks/1
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentMark>> GetById(int id)
    {
        var studentMark = await _context.StudentMarks
    .FromSqlRaw(
        $"SELECT * FROM dbo.get_student_marks_by_id({id})")
    .AsNoTracking()
    .FirstOrDefaultAsync();

        //var student = await _context.StudentMarks.FindAsync(id);

        if (studentMark == null)
            return NotFound(new { message = "Student record not found." });

        return Ok(studentMark);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentMark>>> GetAll()
    {
        var studentMarks = await _context.StudentMarks
            .FromSqlRaw("SELECT * FROM dbo.get_all_student_marks()")
            .AsNoTracking()
            .ToListAsync();

        return Ok(studentMarks);
    }
  

    [HttpPost]
    public async Task<IActionResult> Insert(StudentMark studentMark)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        EXEC dbo.insert_student_marks
            @p_name={studentMark.name},
            @p_subject={studentMark.subject},
            @p_marks={studentMark.marks}
        """);

        return Ok("Student mark inserted successfully.");
    }

    [HttpPut("{id}/marks")]
    public async Task<IActionResult> UpdateMarks(int id, int marks)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        EXEC dbo.update_student_marks
            @p_id={id},
            @p_marks={marks}
        """);

        return Ok("Student mark updated successfully.");
    }
    

    // DELETE: api/student-marks/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        EXEC dbo.delete_student_marks
           @p_id={id}
        """);

        return Ok(new { message = "Student record deleted successfully." });
    }
}