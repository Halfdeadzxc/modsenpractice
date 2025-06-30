using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class RepostCreateDTOValidator : AbstractValidator<RepostCreateDTO>
    {
        public RepostCreateDTOValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("Post ID is required");

            RuleFor(x => x.Comment)
                .MaximumLength(280).WithMessage("Repost comment cannot exceed 280 characters");
        }
    }
}
