using Key2.Models;

namespace Key2.Services
{
    public interface IAdminService
    {
        Task<TokenResponseModel> RegisterAsync(RegisterDeanModel RegisterModel);
        Task<TokenResponseModel> LoginAsync(LoginCredentialsModel model);
        Task<List<Dean>> ShowDeans(Guid Id);
        Task ActiveDean(Guid userId,Guid deanId);

        Task LogOutAsync(TokenBan token);
    }
}
