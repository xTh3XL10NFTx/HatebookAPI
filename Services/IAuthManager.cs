using Hatebook.Models;

namespace Hatebook.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(HatebookLogin request);
        Task<string> CreateToken();
    }
}
