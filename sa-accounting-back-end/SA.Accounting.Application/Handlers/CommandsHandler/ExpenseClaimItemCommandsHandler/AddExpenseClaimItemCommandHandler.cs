using Mapster;
using SA.Accounting.Application.Commands.ExpenseClaimItem;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimItemCommandsHandler;

public class AddExpenseClaimItemCommandHandler(IUnitOfWork unitOfWork,IFileService fileService) : IRequestHandler<AddExpenseClaimItemCommand, Result<ExpenseClaimItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileService _fileService = fileService;
    public async Task<Result<ExpenseClaimItemResponse>> Handle(AddExpenseClaimItemCommand command, CancellationToken cancellationToken)
    {
        // check claim is exist
        if (await _unitOfWork.ExpenseClaims.FindAsync(x => x.Id == command.ClaimId, [],cancellationToken) is not ExpenseClaim claim)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimErrors.NotFound);

        // check claim is (draft or returned for edit)
        if (claim.CurrentState != ExpenseClaimState.Draft && claim.CurrentState != ExpenseClaimState.ReturnedForEdit)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimItemErrors.CannotUpdate);

        // check company is exist
        if (await _unitOfWork.Companies.GetByIdAsync(command.Request.CompanyId,cancellationToken) is not Company company)
            return Result.Failure<ExpenseClaimItemResponse>(CompanyErrors.NotFound);

        // check category is exist
        if (await _unitOfWork.ExpenseCategories.FindAsync(x => x.Id == command.Request.ExpenseCategoryId, [],cancellationToken) is not ExpenseCategory category)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseCategoryErrors.NotFound);

        // check files attachment
        if (category.RequiresAttachment && command.Request.Files is null)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimItemErrors.AttachmentRequired);

        var entity = command.Request.Adapt<ExpenseClaimItem>();

        // upload files
        if (command.Request.Files is not null)
        {
            entity.Attachments = await _fileService.UploadManyAsync(command.Request.Files, cancellationToken);
            foreach (var item in entity.Attachments)
            {
                item.CompanyId = command.Request.CompanyId;
            }
        }

        entity.ExpenseClaimId = command.ClaimId;

        await _unitOfWork.ExpenseClaimItems.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success(entity.Adapt<ExpenseClaimItemResponse>());
    }
}
