using BLL.DTO;
using BLL.Interfaces;
using BLL.Profiles;
using BLL.Services;
using BLL.Validators;
using DAL;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureBLL(this IServiceCollection services, string connection)
        {
            services.ConfigureDAL(connection);
            services.AddAutoMapper(
                typeof(UserProfile),
                typeof(PostProfile),
                typeof(CommentProfile),
                typeof(InteractionProfile),
                typeof(SubscriptionProfile)
             );
            services.AddScoped<IValidator<CommentCreateDTO>, CommentCreateDTOValidator>();
            services.AddScoped<IValidator<CommentDTO>, CommentDTOValidator>();
            services.AddScoped<IValidator<ForgotPasswordDTO>, ForgotPasswordDTOValidator>();
            services.AddScoped<IValidator<LoginDTO>, LoginDTOValidator>();
            services.AddScoped<IValidator<PostCreateDTO>, PostCreateDTOValidator>();
            services.AddScoped<IValidator<PostDTO>, PostDTOValidator>();
            services.AddScoped<IValidator<RegisterDTO>, RegisterDTOValidator>();
            services.AddScoped<IValidator<RepostCreateDTO>, RepostCreateDTOValidator>();
            services.AddScoped<IValidator<ResetPasswordDTO>, ResetPasswordDTOValidator>();
            services.AddScoped<IValidator<UserProfileDTO>, UserProfileDTOValidator>();
            services.AddScoped<IValidator<UserUpdateDTO>, UserUpdateDTOValidator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFeedService, FeedService>();
            services.AddScoped<IInteractionService, InteractionService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
        }
    }
}
