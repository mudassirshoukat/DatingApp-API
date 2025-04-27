using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole, int, IdentityUserClaim<int>,
        AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>

    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        //public DbSet<AppUser> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder Builder)
        {
            base.OnModelCreating(Builder);

            // (UserLike Fluent Validations)

            Builder.Entity<UserLike>().HasKey(x => new { x.SourceUserId, x.TargetUserId });
            Builder.Entity<UserLike>()
                .HasOne(x => x.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(x => x.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            Builder.Entity<UserLike>()
               .HasOne(x => x.TargetUser)
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(x => x.TargetUserId)
               .OnDelete(DeleteBehavior.Cascade);


            // (Message) Fluent Validations

            Builder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.MessagesSent)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            Builder.Entity<Message>()
                .HasOne(x => x.Recipient)
                .WithMany(x => x.MessagesRecieved)
                .HasForeignKey(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
            

            // (AppUser) Fluent Validations

            Builder.Entity<AppUser>().Property(x => x.DateOfBirth).HasConversion<DateOnlyConverter>();


            //(AppUserRole flunt validation)
            Builder.Entity<AppUser>().HasMany(a => a.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired();
            Builder.Entity<AppRole>().HasMany(a => a.UserRoles).WithOne(x => x.Role).HasForeignKey(x => x.RoleId).IsRequired();

            //(connetions fluent validations)
            //Builder.Entity<Connection>().HasKey(x=>x.ConnectionId);
            //Builder.Entity<Connection>().Property(x=>x.ConnectionId).HasColumnType("varchar(255)");


            //(Photos Fluent validation)
            Builder.Entity<Photo>().HasQueryFilter(x => x.IsApproved);

          
           
        
        }



    }
}
