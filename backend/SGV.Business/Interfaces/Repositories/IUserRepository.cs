using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
