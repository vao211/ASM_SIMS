using WebSIMS.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSIMS.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<Users> GetByIdAsync(int id);
        Task<List<Users>> GetAllAsync();
        Task<Users> GetByEmailAsync(string email);
        Task AddAsync(Users users);
        Task UpdateAsync(Users users);
        Task DeleteAsync(int id);
        Task<List<Users>> GetStudentsAsync();
        Task<List<Users>> GetLecturersAsync();
        Task<List<Users>> GetUnassignedUsersByRoleAsync(string role);
    }
}

