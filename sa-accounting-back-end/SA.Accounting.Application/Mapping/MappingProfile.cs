using Mapster;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Contracts.Files.Responses;
using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.ExpenseClaims;

namespace SA.Accounting.Application.Mapping;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Attachment, FileResponse>();

        config.NewConfig<ExpenseClaimItem, ExpenseClaimItemResponse>()
            .Map(dest => dest.Files, src => src.Attachments);
    }
}
