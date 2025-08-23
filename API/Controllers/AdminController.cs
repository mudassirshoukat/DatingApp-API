using API.Data;
using API.DTO.PhotoDtos;
using API.Entities;
using API.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> usermanager;
        private readonly IUnitOfWork unitOfWork;

        public AdminController(UserManager<AppUser> usermanager,IUnitOfWork unitOfWork)
        {
            this.usermanager = usermanager;
            this.unitOfWork = unitOfWork;
        }


        //[Authorize(Policy = "RequireAdminRole")]
        [HttpGet("Users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var user = await usermanager.Users
                .OrderBy(x => x.UserName)
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(x => x.Role.Name).ToList()
                }).ToListAsync();

            return Ok(user);
        }





        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{UserName}")]
        public async Task<ActionResult<IEnumerable<string>>> EditRoles(string UserName, [FromQuery] string Roles)

        {
            if (string.IsNullOrEmpty(Roles)) return BadRequest("You must select atleast one role");
            //var selectedRoles = Roles.Split(",").ToArray();
            var selectedRoles = Roles.Split(",").Select(role => char.ToUpper(role[0]) + role[1..]).ToArray();
            var user = await usermanager.FindByNameAsync(UserName);
            if (user == null) return NotFound();

            var userRoles = await usermanager.GetRolesAsync(user);

            var result = await usermanager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to Add Roles");

            result = await usermanager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to Remove Roles");

            return Ok(await usermanager.GetRolesAsync(user));

        }




    }
}
