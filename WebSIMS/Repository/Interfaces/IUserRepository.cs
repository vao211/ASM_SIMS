using WebSIMS.Models.Entities;

namespace WebSIMS.Repository;

public interface IUserRepository
{
    Task<Users> GetByIdAsync(int id);
    Task<List<Users>> GetAllAsync();
    Task<Users> GetByEmailAsync(string email);
    Task AddAsync(Users users);
    Task UpdateAsync(Users entity);
    Task DeleteAsync(int id);
    Task<List<Users>> GetLecturersAsync();
    Task<List<Users>> GetStudentsAsync();
}