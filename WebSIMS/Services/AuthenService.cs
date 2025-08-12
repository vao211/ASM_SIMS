using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class AuthenService : IAuthenService
{
    private readonly IUserRepository _userRepository;

    public AuthenService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Users> LoginUserAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email.Trim());
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return user;
        }
        return null;
    }

    public async Task<Users> CreateUserAsync(CreateUserViewModel model)
    {
        var user = new Users
        {
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Name = model.Name,
            Role = model.Role
        };
        await _userRepository.AddAsync(user);
        return user;
    }
    public async Task<List<Users>> GetUnassignedUsersByRoleAsync(string role)
    {
        return await _userRepository.GetUnassignedUsersByRoleAsync(role);
    }
}