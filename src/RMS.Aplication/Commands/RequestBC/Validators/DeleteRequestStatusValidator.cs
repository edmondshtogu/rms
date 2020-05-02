using FluentValidation;
using System;

namespace RMS.Application.Commands.RequestBC.Validators
{
    public class DeleteRequestStatusValidator : AbstractValidator<DeleteRequestStatus>
    {
        public DeleteRequestStatusValidator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty);
        }
    }
}
