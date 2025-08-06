using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;

namespace WebSIMS.Repository;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly SIMSDbContext  _context;
    public EnrollmentRepository(SIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollments> GetByIdAsync(int id)
    {
        return await _context.Enrollments.Include(e => e.Student)
            .Include(e => e.Courses)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Enrollments>> GetAllAsync()
    {
        return await _context.Enrollments.Include(e => e.Student)
            .Include(e => e.Courses).ToListAsync();
    }
    public async Task<List<Enrollments>> GetEnrollmentsByStudentAsync(int studentId)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Courses).ThenInclude(c => c.Lecturer)
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Courses).ThenInclude(c => c.Lecturer)
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
    }
    public async Task<Enrollments> GetEnrollmentAsync(int studentId, int courseId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }
    public async Task AddAsync(Enrollments enrollments)
    {
        await _context.Enrollments.AddAsync(enrollments);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollments enrollments)
    {
        _context.Enrollments.Update(enrollments);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var enrollment = await GetByIdAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}