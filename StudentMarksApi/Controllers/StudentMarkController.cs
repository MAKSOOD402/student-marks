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
    .FromSqlInterpolated(
        $"SELECT * FROM get_student_marks_by_id({id})")
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
            .FromSqlRaw("SELECT * FROM public.get_all_student_marks()")
            .AsNoTracking()
            .ToListAsync();

        return Ok(studentMarks);
    }
    /*
    // POST: api/student-marks
    [HttpPost]
    public async Task<ActionResult<StudentMark>> Create(StudentMark student)
    {
        _context.StudentMarks.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = student.id },
            student
        );
    }*/

    [HttpPost]
    public async Task<IActionResult> Insert(StudentMark studentMark)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        CALL public.insert_student_marks(
            {studentMark.name},
            {studentMark.subject},
            {studentMark.marks}
        )
        """);

        return Ok("Student mark inserted successfully.");
    }

    [HttpPut("{id}/marks")]
    public async Task<IActionResult> UpdateMarks(int id, int marks)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        CALL public.update_student_marks(
            {id},
            {marks}
        )
        """);

        return Ok("Student mark updated successfully.");
    }
    

    // DELETE: api/student-marks/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
        CALL public.delete_student_marks(
            {id}
        )
        """);

        return Ok(new { message = "Student record deleted successfully." });
    }
}