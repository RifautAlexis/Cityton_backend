using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cityton.Api.Data;
using System.Linq;
using Cityton.Api.Contracts.Requests;
using Cityton.Api.Contracts.Validators.SharedValidators;

namespace Cityton.Api.Contracts.Validators
{
    public class EditGroupNameRequestValidator : AbstractValidator<EditGroupNameRequest>
    {
        private readonly ApplicationDBContext _appDBContext;

        public EditGroupNameRequestValidator(ApplicationDBContext appDBContext)
        {
            _appDBContext = appDBContext;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(request => request.Id)
                .SetValidator(new IdValidator());
            RuleFor(request => request.GroupName)
                .NotEmpty().WithMessage("Name is empty")
                .Length(3, 50).WithMessage("Have to contains between 3 to 50 characters !")
                .MustAsync(async (name, cancellation) => !(await this.ExistName(name)))
                .WithMessage("{PropertyValue} is already taken !");
        }

        private async Task<bool> ExistName(string name)
        {
            return await _appDBContext.Groups.Where(c => c.Name == name).FirstOrDefaultAsync() != null;
        }
    }
}
