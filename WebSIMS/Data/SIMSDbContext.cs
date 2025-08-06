using WebSIMS.Models.Entities;
namespace WebSIMS.Data;
using Microsoft.EntityFrameworkCore;


public class SIMSDbContext : DbContext
{
    public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options)
    {
        
    }
    public DbSet<Users>  Users { get; set; }
    public DbSet<Enrollments>  Enrollments { get; set; }
    public DbSet<Courses>  Courses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Courses>()
            .HasOne(c => c.Lecturer)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.LecturerId);
        
        modelBuilder.Entity<Enrollments>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);
        
        modelBuilder.Entity<Enrollments>()
            .HasOne(e => e.Courses)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
        
    }
    
}