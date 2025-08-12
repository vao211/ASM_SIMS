using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSIMS.Repository
{
    public class LecturerInforRepository : ILecturerInforRepository
    {
        private readonly SIMSDbContext _context;

        public LecturerInforRepository(SIMSDbContext context)
        {
            _context = context;
        }

        public async Task<LecturerInfor> GetByLecturerIdAsync(string lecturerId)
        {
            return await _context.LecturerInfor.FirstOrDefaultAsync(li => li.LecturerId == lecturerId);
        }

        public async Task<LecturerInfor> GetByIdAsync(int id)
        {
            return await _context.LecturerInfor.FindAsync(id);
        }

        public async Task<List<LecturerInfor>> GetAllAsync()
        {
            return await _context.LecturerInfor.ToListAsync();
        }

        public async Task AddAsync(LecturerInfor lecturerInfor)
        {
            await _context.LecturerInfor.AddAsync(lecturerInfor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LecturerInfor lecturerInfor)
        {
            _context.LecturerInfor.Update(lecturerInfor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var lecturer = await GetByIdAsync(id);
            if (lecturer != null)
            {
                _context.LecturerInfor.Remove(lecturer);
                await _context.SaveChangesAsync();
            }
        }
    }
}