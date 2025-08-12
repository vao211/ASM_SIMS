using WebSIMS.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSIMS.Repository.Interfaces
{
    public interface IStudentInforRepository
    {
        Task<StudentInfor> GetByStudentIdAsync(string studentId);
        Task<StudentInfor> GetByIdAsync(int id);
        Task<List<StudentInfor>> GetAllAsync();
        Task AddAsync(StudentInfor studentInfor);
        Task UpdateAsync(StudentInfor studentInfor);
        Task DeleteAsync(int id);
    }
}