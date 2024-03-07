using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByEmail(string email);
        public Task<User> AddUser(User user);

        public Task<bool> RegisterUser(User user);

        public User CheckCredentials(User user);


    }
}
