using API.Data;
using API.Data.Repository;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.RepoInterfaces;
using API.Services;
using API.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration Config)
        {

            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen();


         var con = Config.GetConnectionString("DefaultConnection");
            Services.AddDbContext<DataContext>(options =>
            {
               options.UseMySql(con, ServerVersion.AutoDetect(con));
            });
            Services.AddCors();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<IPhotoService, PhotoService>();

            Services.AddScoped<IUserRepository, UserRepository>();
            //Services.AddScoped<ILikesRepository, LikesRepository>();
            //Services.AddScoped<IMessageRepository, MessageRepository>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();

            Services.AddAutoMapper(typeof(AutoMapperProfiles));
            Services.Configure<CloudinarySettings>(Config.GetSection("CloudinarySettings"));

            Services.AddScoped<LogUserActivity>();  //actionFilter for LastActive Func
            Services.AddSignalR()
                .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            }); ;
            Services.AddSingleton<PresenceTracker>();


            return Services;
        }
    }
}
