using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Repository.Interfaces;

namespace WebSIMS.Repository;

public class CourseRepository : ICourseRepository
{
    private readonly SIMSDbContext _context;
    private readonly ILogger<CourseRepository> _logger;

    public CourseRepository(SIMSDbContext context, ILogger<CourseRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Courses> GetByIdAsync(int id)
    {
        return await _context.Courses.Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<List<Courses>> GetCoursesByLecturerAsync(int lecturerId)
    {
        return await _context.Courses.Where(c => c.LecturerId == lecturerId).ToListAsync();
    }
    public async Task<List<Courses>> GetAllAsync()
    {
        return await _context.Courses.Include(c => c.Lecturer).ToListAsync();
    }
    
    public async Task AddAsync(Courses courses)
    {
        _logger.LogInformation($"Adding course: Name={courses.Name}, LecturerId={courses.LecturerId}");
        await _context.Courses.AddAsync(courses);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Courses courses)
    {
        _context.Courses.Update(courses);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var course = await GetByIdAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}