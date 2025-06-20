using BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class PostCreateDTOValidator : AbstractValidator<PostCreateDTO>
    {
        public PostCreateDTOValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Post content is required")
                .MaximumLength(280).WithMessage("Post cannot exceed 280 characters");

            RuleFor(x => x.MediaUrls)
                .Must(urls => urls.Count <= 4)
                .WithMessage("Maximum 4 media attachments allowed")
                .When(x => x.MediaUrls != null);


            RuleFor(x => x.Hashtags)
                .MaximumLength(100).WithMessage("Hashtags too long");
        }
    }
}
