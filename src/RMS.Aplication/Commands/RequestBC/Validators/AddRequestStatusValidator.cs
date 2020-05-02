using FluentValidation;

namespace RMS.Application.Commands.RequestBC.Validators
{
    public class AddRequestStatusValidator : AbstractValidator<AddRequestStatus>
    {
        public AddRequestStatusValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
