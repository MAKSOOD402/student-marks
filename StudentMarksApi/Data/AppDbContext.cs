using Microsoft.EntityFrameworkCore;
using StudentMarksApi.Models;

namespace StudentMarksApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudentMark> StudentMarks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentMark>(entity =>
        {
            entity.ToTable("StudentMarks");

            entity.Property(x => x.id).HasColumnName("id");
            entity.Property(x => x.name).HasColumnName("name");
            entity.Property(x => x.subject).HasColumnName("subject");
            entity.Property(x => x.marks).HasColumnName("marks");
        });
    }
}