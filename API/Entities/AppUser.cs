using API.Extentions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
      
    public class AppUser: IdentityUser<int>
    {
        //public int Id { get; set; }
        //public string UserName { get; set; }
        //public Byte[] PasswordHash { get; set; }
        //public Byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction{ get; set; }
        public string LookingFor { get; set; }
        public string Interests{ get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        //navigation properties
        public List<Photo> Photos { get; set; } = new();
        public List<UserLike> LikedUsers { get; set; } = new();
        public List<UserLike> LikedByUsers { get; set; } = new();
        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesRecieved { get; set; }
        public ICollection<AppUserRole> UserRoles  { get; set; }


        public int GetAge { get { return DateOfBirth.CalculateAge(); }  }



    }


}
