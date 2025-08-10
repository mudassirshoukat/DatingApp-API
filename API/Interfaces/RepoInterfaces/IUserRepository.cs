using API.DTO.MemberDtos;
using API.Entities;
using API.Helpers;
using API.Helpers.QueryParams;
using System.Collections.Generic;

namespace API.Interfaces.RepoInterfaces
{
   public interface IUserRepository
   {
      void Update(AppUser user);

      bool UserExists(int id);
      Task<PagedList<AppUser>> GetAllUserAsync(UserQueryParams prms);
      Task<IEnumerable<AppUser>> GetAllUserAsync();
      Task<AppUser> GetUserByIdAsync(int Id);

      Task<AppUser> GetUserByUserNameAsync(string UserName);
      Task<AppUser> GetCurrentUserByUserNameAsync(string UserName);
      Task<string> GetGenderByUserName(string UserName);

      void DeleteUserAsync(AppUser user);

   }
}
