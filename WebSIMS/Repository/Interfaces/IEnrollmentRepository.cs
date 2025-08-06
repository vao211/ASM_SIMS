using WebSIMS.Models.Entities;

namespace WebSIMS.Repository;

public interface IEnrollmentRepository
{
    Task<Enrollments> GetByIdAsync(int id);
    Task<List<Enrollments>> GetAllAsync();
    Task<List<Enrollments>> GetEnrollmentsByStudentAsync(int studentId);
    Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId);
    Task<Enrollments> GetEnrollmentAsync(int studentId, int courseId);
    Task AddAsync(Enrollments entity);
    Task UpdateAsync(Enrollments entity);
    Task DeleteAsync(int id);
}