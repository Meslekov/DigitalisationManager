namespace DigitalisationManager.Services.Core
{
    using System.Security.Cryptography;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using DigitalisationManager.Data;
    using DigitalisationManager.Data.Models.Entities;
    using DigitalisationManager.GCommon.Enums;
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Services.Core.Options;
    using DigitalisationManager.Web.ViewModels.DigitalFile;

    public class DigitalFileService : IDigitalFileService
    {
        private static readonly HashSet<string> AllowedExt =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".tif",
                ".tiff"
            };

        private readonly DigitalisationManagerDbContext context;
        private readonly IFileStorageService fileStorage;
        private readonly FileStorageOptions options;

        public DigitalFileService(
            DigitalisationManagerDbContext context,
            IFileStorageService fileStorage,
            IOptions<FileStorageOptions> options)
        {
            this.context = context;
            this.fileStorage = fileStorage;
            this.options = options.Value;
        }

        public async Task<IReadOnlyList<DigitalFileListViewModel>> ListByItemAsync(int itemId)
        {
            return await context.DigitalFiles
                .AsNoTracking()
                .Where(df => df.ItemId == itemId)
                .OrderByDescending(df => df.UploadedAt)
                .Select(df => new DigitalFileListViewModel
                {
                    Id = df.Id,
                    OriginalFileName = df.OriginalFileName,
                    OriginalContentType = df.OriginalContentType,
                    OriginalSizeBytes = df.OriginalSizeBytes,
                    PreviewContentType = df.PreviewContentType,
                    PreviewSizeBytes = df.PreviewSizeBytes,
                    IsDownloadAllowed = df.IsDownloadAllowed,
                    UploadedAt = df.UploadedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string? Error)> UploadAsync(int itemId, IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return (false, "Please choose a file.");
            }

            if (file.Length > options.MaxTiffUploadSizeBytes)
            {
                return (false, $"File is too large. Max allowed is {options.MaxTiffUploadSizeBytes:N0} bytes.");
            }

            string extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExt.Contains(extension))
            {
                return (false, "Only .tif / .tiff files are allowed.");
            }

            Item? item = await context.Items
                .Include(i => i.DigitalFiles)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item is null)
            {
                return (false, "Item not found.");
            }

            string normalizedExtension = extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase)
                ? ".tiff"
                : ".tif";

            string originalStoredName = $"{Guid.NewGuid():N}{normalizedExtension}";
            string originalRelativePath = $"Funds/{item.FundId}/Items/{item.Id}/Originals/{originalStoredName}";

            await using MemoryStream memoryStream = new MemoryStream();
            await using Stream inputStream = file.OpenReadStream();
            await inputStream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            string sha256 = ComputeSha256Hex(memoryStream);

            memoryStream.Position = 0;
            await fileStorage.SaveAsync(originalRelativePath, memoryStream);

          
            DigitalFile entity = new DigitalFile
            {
                ItemId = item.Id,
                OriginalFileName = Path.GetFileName(file.FileName),
                OriginalStoredFileName = originalStoredName,
                OriginalRelativePath = originalRelativePath,
                OriginalContentType = "image/tiff",
                OriginalSizeBytes = file.Length,
                OriginalChecksumSha256 = sha256,
                PreviewStoredFileName = originalStoredName,
                PreviewRelativePath = originalRelativePath,
                PreviewContentType = "image/tiff",
                PreviewSizeBytes = file.Length,
                IsDownloadAllowed = false,
                UploadedAt = DateTime.UtcNow
            };

            context.DigitalFiles.Add(entity);

            if (!item.DigitalFiles.Any())
            {
                item.Status = ItemStatus.Digitized;
            }

            await context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(byte[] Content, string ContentType, string DownloadName)?> DownloadOriginalAsync(int id)
        {
            DigitalFile? digitalFile = await context.DigitalFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(df => df.Id == id);

            if (digitalFile is null)
            {
                return null;
            }

            if (!fileStorage.Exists(digitalFile.OriginalRelativePath))
            {
                return null;
            }

            await using Stream stream = fileStorage.OpenRead(digitalFile.OriginalRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.OriginalContentType,
                digitalFile.OriginalFileName
            );
        }

        public async Task<(byte[] Content, string ContentType, string DownloadName)?> DownloadPreviewAsync(int id)
        {
            DigitalFile? digitalFile = await context.DigitalFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(df => df.Id == id);

            if (digitalFile is null)
            {
                return null;
            }

            if (!fileStorage.Exists(digitalFile.PreviewRelativePath))
            {
                return null;
            }

            string previewDownloadName = Path.GetFileNameWithoutExtension(digitalFile.OriginalFileName) + ".jpg";

            await using Stream stream = fileStorage.OpenRead(digitalFile.PreviewRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.PreviewContentType,
                previewDownloadName
            );
        }

        public async Task<(byte[] Content, string ContentType)?> GetPreviewImageAsync(int id)
        {
            DigitalFile? digitalFile = await context.DigitalFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(df => df.Id == id);

            if (digitalFile is null)
            {
                return null;
            }

            if (!fileStorage.Exists(digitalFile.PreviewRelativePath))
            {
                return null;
            }

            await using Stream stream = fileStorage.OpenRead(digitalFile.PreviewRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.PreviewContentType
            );
        }

        public async Task<DigitalFilePreviewViewModel?> GetPreviewPageAsync(
            int id,
            bool canDownloadOriginal,
            bool canDownloadPreview)
        {
            DigitalFile? currentFile = await context.DigitalFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(df => df.Id == id);

            if (currentFile is null)
            {
                return null;
            }

            List<DigitalFile> files = await context.DigitalFiles
                .AsNoTracking()
                .Where(df => df.ItemId == currentFile.ItemId)
                .OrderBy(df => df.UploadedAt)
                .ThenBy(df => df.Id)
                .ToListAsync();

            int index = files.FindIndex(df => df.Id == id);
            if (index < 0)
            {
                return null;
            }

            return new DigitalFilePreviewViewModel
            {
                Id = currentFile.Id,
                ItemId = currentFile.ItemId,
                OriginalFileName = currentFile.OriginalFileName,
                PreviewImageUrl = string.Empty,
                Position = index + 1,
                TotalCount = files.Count,
                PreviousFileId = index > 0 ? files[index - 1].Id : null,
                NextFileId = index < files.Count - 1 ? files[index + 1].Id : null,
                CanDownloadOriginal = canDownloadOriginal,
                CanDownloadPreview = canDownloadPreview
            };
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            DigitalFile? digitalFile = await context.DigitalFiles
                .FirstOrDefaultAsync(df => df.Id == id);

            if (digitalFile is null)
            {
                return (false, "File not found.");
            }

            if (fileStorage.Exists(digitalFile.OriginalRelativePath))
            {
                fileStorage.Delete(digitalFile.OriginalRelativePath);
            }

            bool previewIsSeparateFile =
                !string.Equals(
                    digitalFile.PreviewRelativePath,
                    digitalFile.OriginalRelativePath,
                    StringComparison.OrdinalIgnoreCase);

            if (previewIsSeparateFile && fileStorage.Exists(digitalFile.PreviewRelativePath))
            {
                fileStorage.Delete(digitalFile.PreviewRelativePath);
            }

            context.DigitalFiles.Remove(digitalFile);
            await context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> SetDownloadAllowedAsync(int id, bool isAllowed)
        {
            DigitalFile? digitalFile = await context.DigitalFiles
                .FirstOrDefaultAsync(df => df.Id == id);

            if (digitalFile is null)
            {
                return (false, "File not found.");
            }

            digitalFile.IsDownloadAllowed = isAllowed;
            await context.SaveChangesAsync();

            return (true, null);
        }

        private static string ComputeSha256Hex(Stream stream)
        {
            stream.Position = 0;

            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(stream);

            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}