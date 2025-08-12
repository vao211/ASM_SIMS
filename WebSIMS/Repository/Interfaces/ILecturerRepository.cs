using WebSIMS.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSIMS.Repository.Interfaces
{
    public interface ILecturerInforRepository
    {
        Task<LecturerInfor> GetByLecturerIdAsync(string lecturerId);
        Task<LecturerInfor> GetByIdAsync(int id);
        Task<List<LecturerInfor>> GetAllAsync();
        Task AddAsync(LecturerInfor lecturerInfor);
        Task UpdateAsync(LecturerInfor lecturerInfor);
        Task DeleteAsync(int id);
    }
}