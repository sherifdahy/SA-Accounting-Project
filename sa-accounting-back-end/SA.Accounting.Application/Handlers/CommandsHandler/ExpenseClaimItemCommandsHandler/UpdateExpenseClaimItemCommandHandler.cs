using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.ExpenseClaimItem;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimItemCommandsHandler;

public class UpdateExpenseClaimItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateExpenseClaimItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(UpdateExpenseClaimItemCommand command, CancellationToken cancellationToken)
    {
        // check claim item is exist
        if (await _unitOfWork.ExpenseClaimItems.FindAsync(x => x.Id == command.ClaimItemId, [x=>x.Include(x=>x.ExpenseClaim)], cancellationToken) is not ExpenseClaimItem claimItem)
            return Result.Failure(ExpenseClaimItemErrors.NotFound);

        // check claim is (draft or returned for edit)
        if (claimItem.ExpenseClaim.CurrentState != ExpenseClaimState.Draft && claimItem.ExpenseClaim.CurrentState != ExpenseClaimState.ReturnedForEdit)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimItemErrors.CannotUpdate);

        // check company is exist
        if (await _unitOfWork.Companies.GetByIdAsync(command.Request.CompanyId, cancellationToken) is not Company company)
            return Result.Failure<ExpenseClaimItemResponse>(CompanyErrors.NotFound);

        // check category is exist
        if (await _unitOfWork.ExpenseCategories.FindAsync(x => x.Id == command.Request.ExpenseCategoryId, [], cancellationToken) is not ExpenseCategory category)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseCategoryErrors.NotFound);


        // check files attachment
        if (category.RequiresAttachment && command.Request.Files is null)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimItemErrors.AttachmentRequired);

        command.Request.Adapt(claimItem);

        if (category.RequiresAttachment)
        {
            // upload files

            //entity.FilesUrl.AddRange();

            //entity.FilesUrls = string.Join(',', command.Request.Files.Select(x => x.Name));
        }

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
