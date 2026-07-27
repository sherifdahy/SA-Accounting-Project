using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Contracts.ExpenseClaims.Responses;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;

public class UpdateExpenseClaimHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateExpenseClaimCommand, Result<ExpenseClaimResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<ExpenseClaimResponse>> Handle(UpdateExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        var request = command.Request;

        //var claim = await _unitOfWork.ExpenseClaims
        //    .FindAsync(x => x.Id == command.Id, [x=>x.Include(d=>d.Items)], cancellationToken);

        //if (claim is null)
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.NotFound);

        //// 1. State check
        //if (claim.CurrentState != ExpenseClaimState.Draft &&
        //    claim.CurrentState != ExpenseClaimState.ReturnedForEdit)
        //{
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.NotEditable);
        //}

        //// 2. Validate companies
        //var companyIds = request.Items.Select(x => x.CompanyId).Distinct().ToList();
        //var existingCompanies = (await _unitOfWork.Companies
        //    .FindAllAsync(x => companyIds.Contains(x.Id), [], cancellationToken))
        //    .Select(x => x.Id).ToList();

        //if (existingCompanies.Count != companyIds.Count)
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.CompanyNotFound);

        //// 3. Validate categories
        //var categoryIds = request.Items.Select(x => x.ExpenseCategoryId).Distinct().ToList();
        //var categories = await _unitOfWork.ExpenseCategories
        //    .FindAllAsync(x => categoryIds.Contains(x.Id), [], cancellationToken) ;

        //if (categories.Count() != categoryIds.Count)
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseCategoryErrors.NotFound);

        //if (categories.Any(c => c.IsDisabled))
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.InactiveCategory);

        //// 4. RequiresAttachment validation
        //var categoryDict = categories.ToDictionary(c => c.Id);
        //foreach (var itemReq in request.Items)
        //{
        //    var category = categoryDict[itemReq.ExpenseCategoryId];
        //    if (category.RequiresAttachment && string.IsNullOrWhiteSpace(itemReq.FileUrl))
        //        return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.AttachmentRequired);
        //}

        //// 5. Validate that any provided item Ids actually belong to this claim
        //var existingItemIds = claim.Items.Select(i => i.Id).ToHashSet();
        //var providedItemIds = request.Items
        //    .Where(i => i.Id.HasValue)
        //    .Select(i => i.Id!.Value)
        //    .ToList();

        //if (providedItemIds.Any(id => !existingItemIds.Contains(id)))
        //    return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.InvalidItemReference);

        //// 6. Apply changes to header
        ////claim.ClaimDate = request.ClaimDate;
        //claim.Note = request.Note ?? string.Empty;
        //// ملحوظة: مش بنغير الـ UserId في Update

        //// 7. Sync items: Add / Update / Remove
        //// 7a. Remove items not in request
        //var keepIds = providedItemIds.ToHashSet();
        //var toRemove = claim.Items.Where(i => !keepIds.Contains(i.Id)).ToList();
        //foreach (var item in toRemove)
        //    _unitOfWork.ExpenseClaimItems.Delete(item);

        //// 7b. Update existing
        //foreach (var itemReq in request.Items.Where(i => i.Id.HasValue))
        //{
        //    var existing = claim.Items.First(i => i.Id == itemReq.Id);
        //    existing.CompanyId = itemReq.CompanyId;
        //    existing.ExpenseCategoryId = itemReq.ExpenseCategoryId;
        //    existing.Note = itemReq.Note;
        //    existing.FileUrl = itemReq.FileUrl;
        //    existing.Amount = itemReq.Amount;
        //    // الـ State يفضل Pending لو رجع للتعديل (مش بنغيره هنا)
        //}

        //// 7c. Add new
        //foreach (var itemReq in request.Items.Where(i => !i.Id.HasValue))
        //{
        //    claim.Items.Add(new ExpenseClaimItem
        //    {
        //        CompanyId = itemReq.CompanyId,
        //        ExpenseCategoryId = itemReq.ExpenseCategoryId,
        //        Note = itemReq.Note,
        //        FileUrl = itemReq.FileUrl,
        //        Amount = itemReq.Amount,
        //        State = ExpenseClaimItemState.Pending
        //    });
        //}

        //await _unitOfWork.SaveAsync(cancellationToken);

        //var saved = await _unitOfWork.ExpenseClaims
        //    .FindAsync(x => x.Id == claim.Id, 
        //    [
        //        x=>x.Include(i=>i.User),
        //        x=>x.Include(i=>i.Items).ThenInclude(d=>d.Company),
        //        x=>x.Include(i=>i.Items).ThenInclude(d=>d.ExpenseCategory),
        //        x=>x.Include(i=>i.Histories).ThenInclude(d=>d.CreatedBy)
        //    ], cancellationToken);

        //return Result.Success(null);

        throw new NotImplementedException();
    }
}