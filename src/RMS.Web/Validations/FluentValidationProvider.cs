using FluentValidation;
using FluentValidation.Results;
using RMS.Core;
using RMS.Core.Domain;
using RMS.Messages;
using RMS.Messages.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RequestsManagementSystem.Validations
{
    public class FluentValidationProvider : ICommandValidationProvider
    {
        private static ValidationResponse BuildValidationResponse(ValidationResult validationResult)
        {
            return new ValidationResponse
            {
                Errors = validationResult.Errors.Select(failure => new ValidationError
                {
                    PropertyName = failure.PropertyName,
                    ErrorMessage = failure.ErrorMessage
                }).ToList()
            };
        }

        public async Task<ValidationResponse> ValidateAsync<TCommandResult>(ICommand<TCommandResult> command)
        {
            var validator = EngineContext.Current.Resolve(command, typeof(IValidator<>));
            if (validator == null)
            {
                // No validator found!
                return new ValidationResponse();
            }
            var validateMethod = validator.GetType().GetMethod("ValidateAsync", new[] { command.GetType(), typeof(CancellationToken) });
            var validationResult = await (Task<ValidationResult>)validateMethod.Invoke(validator, new object[] { command, default(CancellationToken) });

            return BuildValidationResponse(validationResult);
        }
    }
}