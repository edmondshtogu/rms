using FluentValidation;

namespace RMS.Application.Commands.RequestBC.Validators
{
    public class CheckIfRequestExistValidator : AbstractValidator<CheckIfRequestExist>
    {
        public CheckIfRequestExistValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RaisedDate).NotEmpty();
        }
    }
}
