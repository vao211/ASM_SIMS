using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;

namespace WebSIMS.Repository;

public class UserRepository : IUserRepository
{
    private readonly SIMSDbContext _context;

    public UserRepository(SIMSDbContext context)
    {
        _context = context;
    }
    public async Task<Users> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<Users>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<Users> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(Users users)
    {
        await _context.Users.AddAsync(users);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Users users)
    {
        _context.Users.Update(users);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Users>> GetStudentsAsync()
    {
        return await _context.Users.Where(u=> u.Role == "Student").ToListAsync();
    }
    public async Task<List<Users>> GetLecturersAsync()
    {
        return await _context.Users.Where(u => u.Role == "Lecturer").ToListAsync();
    }
}