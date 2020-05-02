using FluentValidation;
using System;

namespace RMS.Application.Commands.RequestBC.Validators
{
    public class DeleteRequestValidator : AbstractValidator<DeleteRequest>
    {
        public DeleteRequestValidator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty);
        }
    }
}
