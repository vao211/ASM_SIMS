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
    
    public DbSet<StudentInfor> StudentInfor { get; set; }
    
    public DbSet<LecturerInfor> LecturerInfor { get; set; }
    
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
        
        
        // Cấu hình mối quan hệ cho StudentInfor (không bắt buộc khóa ngoại)
        modelBuilder.Entity<StudentInfor>()
            .HasOne(si => si.User)
            .WithOne()
            .HasForeignKey<StudentInfor>(si => si.UserId)
            .IsRequired(false); // UserId có thể null

        // Cấu hình mối quan hệ cho LecturerInfor (không bắt buộc khóa ngoại)
        modelBuilder.Entity<LecturerInfor>()
            .HasOne(li => li.User)
            .WithOne()
            .HasForeignKey<LecturerInfor>(li => li.UserId)
            .IsRequired(false); // UserId có thể null
    }
    
}