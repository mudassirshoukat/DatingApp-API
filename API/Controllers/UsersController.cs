using API.DTO.MemberDtos;
using API.DTO.PhotoDtos;
using API.DTO.UserDtos;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Helpers.QueryParams;
using API.Interfaces;
using API.Interfaces.RepoInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{

   //[Authorize]
   public class UsersController : BaseApiController
   {
      private readonly IUnitOfWork unitOfWork;
      private readonly IMapper mapper;
      private readonly IUserRepository userRepo;
      private readonly IPhotoService _photoService;

      public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepo)
      {
         this.unitOfWork = unitOfWork;
         this.mapper = mapper;
         this.userRepo = userRepo;
      }


      [HttpGet]
      [Route("Get-List-All")]
      public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetAllUsers()
      {

         var users = await unitOfWork.UserRepository.GetAllUserAsync();

         var mappedUsers = mapper.Map<IEnumerable<MemberResponseDto>>(users);



         return Ok(mappedUsers);
      }



      [HttpGet]
      public async Task<ActionResult<PagedList<MemberResponseDto>>> GetUsers([FromQuery] UserQueryParams prms)
      {
         var gender = await unitOfWork.UserRepository.GetGenderByUserName(User.GetUserName());
         prms.CurrentUserName = User.GetUserName();
         if (string.IsNullOrEmpty(prms.Gender))
         {
            prms.Gender = gender == "male" ? "female" : "male";
         }


         PagedList<AppUser> users = await unitOfWork.UserRepository.GetAllUserAsync(prms);

         var mappedUsers = mapper.Map<IEnumerable<MemberResponseDto>>(users);

         var mappedPagedList = new PagedList<MemberResponseDto>(
             mappedUsers,
             users.TotalCount,
             users.CurrentPage,
             users.PageSize
         );

         Response.AddPaginationHeader(
             new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

         return Ok(mappedPagedList);
      }




      [HttpGet()]
      [Route("{UserName}")]
      public async Task<ActionResult<MemberResponseDto>> GetUserByUserName(string UserName)
      {

         var appUser = new AppUser();

         if (UserName.ToLower() == User.GetUserName().ToLower())

            appUser = await unitOfWork.UserRepository.GetCurrentUserByUserNameAsync(UserName);

         else
            appUser = await unitOfWork.UserRepository.GetUserByUserNameAsync(UserName);

         if (appUser == null)
         {
            return NotFound();
         }

         return Ok(mapper.Map<MemberResponseDto>(appUser));
      }




      [HttpPut()]
      public async Task<IActionResult> PutUser(MemberUpdateRequestDto member)
      {
         string userName = User.GetUserName();
         AppUser user = await unitOfWork.UserRepository.GetUserByUserNameAsync(userName);
         mapper.Map(member, user);

         if (await unitOfWork.Complete()) return NoContent();
         return BadRequest("Failed to update resource");

      }

      // POST: api/Users
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      //[HttpPost]
      //public async Task<ActionResult<AppUser>> PostUser([FromBody] AppUser appUser)
      //{
      //    _context.Users.Add(appUser);
      //    await _context.SaveChangesAsync();

      //    return CreatedAtAction("GetAppUser", new { id = appUser.id }, appUser);
      //}

      // DELETE: api/Users/5
      [HttpDelete()]
      [Route("{id}")]
      public async Task<IActionResult> DeleteUser([FromRoute] int id)
      {
         var appUser = await unitOfWork.UserRepository.GetUserByIdAsync(id);
         if (appUser == null)
         {

            return NotFound();
         }

         UserResponseDto dto = new UserResponseDto();

         unitOfWork.UserRepository.DeleteUserAsync(appUser);
         await unitOfWork.Complete();

         return NoContent();
      }


   }


}
