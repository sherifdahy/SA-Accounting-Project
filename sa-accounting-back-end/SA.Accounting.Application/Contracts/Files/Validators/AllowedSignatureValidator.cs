using Microsoft.AspNetCore.Http;
using SA.Accounting.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Files.Validators;

public class AllowedSignatureValidator : AbstractValidator<IFormFile>
{
    public AllowedSignatureValidator()
    {
        RuleFor(x => x)
            .Must((request, context) =>
            {
                BinaryReader binaryReader = new BinaryReader(request.OpenReadStream());
                var bytes = binaryReader.ReadBytes(2);
                var fileSequenceHex = BitConverter.ToString(bytes);

                if (!FileSettings.AllowedSignatures.Contains(fileSequenceHex))
                    return false;

                return true;
            }).WithMessage("Only valid PDF files are allowed.").When(x => x is not null); ;
    }
}
