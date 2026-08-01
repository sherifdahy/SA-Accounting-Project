using SA.Accounting.Application.Commands.User;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.UserCommandsHandler;

public class RemoveCompanyFromUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveCompanyFromUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(RemoveCompanyFromUserCommand request, CancellationToken cancellationToken)
    {
        var companyUser = await _unitOfWork.UserCompanies.FindAsync(c => c.CompanyId == request.CompanyId && c.UserId == request.UserId, cancellationToken);

        if (companyUser is null)
            return Result.Failure(UserCompanyErrors.NotFound);

        _unitOfWork.UserCompanies.Delete(companyUser);
        _unitOfWork.Save();

        return Result.Success();
    }
}
