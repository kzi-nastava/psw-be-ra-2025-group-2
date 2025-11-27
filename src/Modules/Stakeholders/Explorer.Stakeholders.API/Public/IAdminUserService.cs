using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IAdminUserService
{
    public void CreateAccount(AccountRegistrationDto account);
    public void BlockUser(string username);
    public void UnblockUser(string username);
    public AdminUserInfoDto GetUserInfoByName(string username);
    public PagedResult<AdminUserInfoDto> GetUsers(int pageNumber, int pageSize);
}
