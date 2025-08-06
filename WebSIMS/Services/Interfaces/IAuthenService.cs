using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;

namespace WebSIMS.Services.Interfaces;

public interface IAuthenService
{
    Task<Users> LoginUserAsync(string email, string password);
    Task<Users> CreateUserAsync(CreateUserViewModel model);
}