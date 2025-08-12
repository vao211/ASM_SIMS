using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSIMS.Repository
{
    public class StudentInforRepository : IStudentInforRepository
    {
        private readonly SIMSDbContext _context;

        public StudentInforRepository(SIMSDbContext context)
        {
            _context = context;
        }

        public async Task<StudentInfor> GetByStudentIdAsync(string studentId)
        {
            return await _context.StudentInfor.FirstOrDefaultAsync(si => si.StudentId == studentId);
        }

        public async Task<StudentInfor> GetByIdAsync(int id)
        {
            return await _context.StudentInfor.FindAsync(id);
        }

        public async Task<List<StudentInfor>> GetAllAsync()
        {
            return await _context.StudentInfor.ToListAsync();
        }

        public async Task AddAsync(StudentInfor studentInfor)
        {
            await _context.StudentInfor.AddAsync(studentInfor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentInfor studentInfor)
        {
            _context.StudentInfor.Update(studentInfor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await GetByIdAsync(id);
            if (student != null)
            {
                _context.StudentInfor.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}