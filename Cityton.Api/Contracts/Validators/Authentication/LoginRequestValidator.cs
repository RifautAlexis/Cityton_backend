using FluentValidation;
using Cityton.Api.Contracts.Requests;

namespace Cityton.Api.Contracts.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {

        public LoginRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(request => request.loginDTO).NotNull();
            When(request => request.loginDTO != null,
                () => { RuleFor(request => request.loginDTO).SetValidator(new LoginDTOValidator()); });
        }
    }
}
