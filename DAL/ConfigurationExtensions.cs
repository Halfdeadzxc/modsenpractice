using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureDAL(
            this IServiceCollection services, string connection)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            services.AddScoped<IFeedRepository,FeedRepository>();
            services.AddScoped<ILikeRepository,  LikeRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IRepostRepository,  RepostRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            
        }
    }
}
