using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class CommentDTOValidator : AbstractValidator<CommentDTO>
    {
        public CommentDTOValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Comment ID is required");

            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("Post ID is required");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author ID is required");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(280).WithMessage("Comment cannot exceed 280 characters");
        }
    }
}
