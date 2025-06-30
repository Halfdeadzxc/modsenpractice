using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class CommentCreateDTOValidator : AbstractValidator<CommentCreateDTO>
    {
        public CommentCreateDTOValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("Post ID is required");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(280).WithMessage("Comment cannot exceed 280 characters");
        }
    }
}
