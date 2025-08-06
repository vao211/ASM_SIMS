using WebSIMS.Models.Entities;

namespace WebSIMS.Repository.Interfaces;

public interface ICourseRepository
{
    Task<Courses> GetByIdAsync(int id);
    Task<List<Courses>> GetAllAsync();
    Task<List<Courses>> GetCoursesByLecturerAsync(int instructorId);
    Task AddAsync(Courses entity);
    Task UpdateAsync(Courses entity);
    Task DeleteAsync(int id);
}