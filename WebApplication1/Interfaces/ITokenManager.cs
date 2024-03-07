using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface ITokenManager
    {
        string GenerateToken(User user);
    }
}
