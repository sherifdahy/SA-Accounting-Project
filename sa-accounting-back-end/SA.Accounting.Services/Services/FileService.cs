using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Services.Services;

public class FileService(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork) : IFileService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/files";
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<(byte[] fileContent, string contentType, string fileName)> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _unitOfWork.Attachments.FindAsync(x => x.Id == id, [], cancellationToken);

        if (file is null)
            return ([], string.Empty, string.Empty);

        var path = Path.Combine(_filesPath, file.StoredFileName);

        using MemoryStream memoryStream = new();
        using FileStream fileStream = new(path, FileMode.Open);
        await fileStream.CopyToAsync(memoryStream, cancellationToken);

        return (memoryStream.ToArray(), file.ContentType, file.FileName);
    }

    public async Task<List<Attachment>> UploadManyAsync(IFormFileCollection formFiles, CancellationToken cancellationToken = default)
    {
        var attachments = new List<Attachment>();

        foreach (var file in formFiles)
        {
            attachments.Add(await SaveFileAsync(file, cancellationToken));
        }

        return attachments;
    }

    private async Task<Attachment> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var randomFileName = Path.GetRandomFileName();

        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoredFileName = randomFileName,
            ContentType = file.ContentType,
            FileExtension = Path.GetExtension(file.FileName)
        };

        Directory.CreateDirectory(_filesPath);

        var path = Path.Combine(_filesPath, attachment.StoredFileName);

        using Stream stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return attachment;
    }
}
