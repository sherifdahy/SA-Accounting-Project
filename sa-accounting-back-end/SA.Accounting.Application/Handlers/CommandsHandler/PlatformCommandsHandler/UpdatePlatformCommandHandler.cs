using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.Platform;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Entities.Platforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.PlatformCommandsHandler;

public class UpdatePlatformCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdatePlatformCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdatePlatformCommand request, CancellationToken cancellationToken)
    {
        var platform = await _unitOfWork.Platforms.FindAsync(
            x => x.Id == request.Id,
            cancellationToken,
            x => x.Selectors);

        if (platform == null)
            return Result.Failure(PlatformErrors.NotFound);


        platform.Name = request.Name;
        platform.Url = request.Url;
        platform.ImageUrl = request.ImageUrl;

        #region Selectors

        // Add new selectors
        var newSelectors = request.Selectors
            .Where(x => x.Id == 0);

        await _unitOfWork.Selectors.AddRangeAsync(newSelectors.Adapt<List<Selector>>(), cancellationToken);

        // Update existing selectors
        var updateSelectors = request.Selectors
            .Where(x => x.Id != 0);

        foreach(var selector in updateSelectors)
        {
            var existingSelector = platform.Selectors.First(x=>x.Id == selector.Id);

            selector.Adapt(existingSelector);
        }

        // Delete removed selectors
        var deletedSelectors = platform.Selectors
            .Where(d => !request.Selectors.Where(x=>x.Id != 0).Select(w=>w.Id).ToHashSet().Contains(d.Id));

        _unitOfWork.Selectors.DeleteRange(deletedSelectors);

        #endregion

        await _unitOfWork.SaveAsync(cancellationToken);
        return Result.Success();
    }
}
