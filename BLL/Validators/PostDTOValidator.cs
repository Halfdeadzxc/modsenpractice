using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class PostDTOValidator : AbstractValidator<PostDTO>
    {
        public PostDTOValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Post ID is required");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author ID is required");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Post content is required")
                .MaximumLength(280).WithMessage("Post cannot exceed 280 characters");
        }
    }
}
