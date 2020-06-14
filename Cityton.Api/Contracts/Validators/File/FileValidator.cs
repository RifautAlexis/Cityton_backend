using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Cityton.Api.Contracts.Validators.File
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(file => file.Length).NotNull().LessThanOrEqualTo(100)
                .WithMessage("File size is larger than allowed");
            RuleFor(file => file.ContentType).NotNull().Must(type => FileConstants._contentType.Contains(type))
                .WithMessage("This file type is not allowed");
            RuleFor(file => file)
                .Custom((file, context) =>
                {

                    var reader = new BinaryReader(file.OpenReadStream());
                    var signatures = FileConstants._fileSignature[file.ContentType];
                    var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                    bool singatureIsCorrect = signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
                    if (singatureIsCorrect) context.AddFailure("Header's bytes do not correspond to the mime");
                });
        }
    }
}