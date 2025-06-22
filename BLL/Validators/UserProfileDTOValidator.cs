using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class UserProfileDTOValidator : AbstractValidator<UserProfileDTO>
    {
        public UserProfileDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(30).WithMessage("Username cannot exceed 30 characters")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers and underscores");

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(200).WithMessage("Avatar URL too long")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl))
                .WithMessage("Invalid avatar URL format");

            RuleFor(x => x.Bio)
                .MaximumLength(150).WithMessage("Bio cannot exceed 150 characters");
        }
    }
}
