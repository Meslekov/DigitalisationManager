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
        private readonly IOriginalFileStorageService originalFileStorageService;
        private readonly IPreviewImageStorageService previewImageStorageService;
        private readonly ITiffConversionService tiffConversionService;
        private readonly FileStorageOptions options;

        public DigitalFileService(
            DigitalisationManagerDbContext context,
            IOriginalFileStorageService originalFileStorageService,
            IPreviewImageStorageService previewImageStorageService,
            ITiffConversionService tiffConversionService,
            IOptions<FileStorageOptions> options)
        {
            this.context = context;
            this.originalFileStorageService = originalFileStorageService;
            this.previewImageStorageService = previewImageStorageService;
            this.tiffConversionService = tiffConversionService;
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

        public async Task<BatchDigitalFileUploadResultViewModel> UploadAsync(int itemId, IEnumerable<IFormFile> files)
        {
            BatchDigitalFileUploadResultViewModel batchResult = new BatchDigitalFileUploadResultViewModel();

            Item? item = await context.Items
                .Include(i => i.DigitalFiles)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item is null)
            {
                batchResult.TotalCount = 0;
                batchResult.SuccessCount = 0;
                batchResult.FailedCount = 1;
                batchResult.Results.Add(new DigitalFileUploadResultViewModel
                {
                    FileName = $"Item {itemId}",
                    Success = false,
                    Error = "Item not found."
                });

                return batchResult;
            }

            List<IFormFile> fileList = files?
                .Where(f => f is not null)
                .ToList() ?? new List<IFormFile>();

            batchResult.TotalCount = fileList.Count;

            if (fileList.Count == 0)
            {
                batchResult.FailedCount = 1;
                batchResult.Results.Add(new DigitalFileUploadResultViewModel
                {
                    FileName = "No files selected",
                    Success = false,
                    Error = "Please choose at least one TIFF file."
                });

                return batchResult;
            }

            foreach (IFormFile file in fileList)
            {
                DigitalFileUploadResultViewModel fileResult = new DigitalFileUploadResultViewModel
                {
                    FileName = file.FileName
                };

                string? savedOriginalPath = null;
                string? savedPreviewPath = null;

                try
                {
                    string? validationError = ValidateUploadFile(file);
                    if (validationError is not null)
                    {
                        fileResult.Success = false;
                        fileResult.Error = validationError;
                        batchResult.Results.Add(fileResult);
                        batchResult.FailedCount++;
                        continue;
                    }

                    string extension = Path.GetExtension(file.FileName);
                    string normalizedExtension = extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase)
                        ? ".tiff"
                        : ".tif";

                    await using MemoryStream originalBuffer = new MemoryStream();
                    await using Stream inputStream = file.OpenReadStream();
                    await inputStream.CopyToAsync(originalBuffer);

                    originalBuffer.Position = 0;
                    string sha256 = ComputeSha256Hex(originalBuffer);

                    originalBuffer.Position = 0;
                    byte[] previewJpegBytes = await tiffConversionService.ConvertFirstPageToJpegAsync(originalBuffer);

                    originalBuffer.Position = 0;
                    var originalStorageResult = await originalFileStorageService.SaveAsync(
                        item.FundId,
                        item.Id,
                        normalizedExtension,
                        originalBuffer);

                    savedOriginalPath = originalStorageResult.RelativePath;

                    await using MemoryStream previewBuffer = new MemoryStream(previewJpegBytes);
                    var previewStorageResult = await previewImageStorageService.SaveAsync(
                        item.FundId,
                        item.Id,
                        previewBuffer);

                    savedPreviewPath = previewStorageResult.RelativePath;

                    DigitalFile digitalFile = new DigitalFile
                    {
                        ItemId = item.Id,
                        OriginalFileName = Path.GetFileName(file.FileName),
                        OriginalStoredFileName = originalStorageResult.StoredFileName,
                        OriginalRelativePath = originalStorageResult.RelativePath,
                        OriginalContentType = "image/tiff",
                        OriginalSizeBytes = originalStorageResult.SizeBytes,
                        OriginalChecksumSha256 = sha256,
                        PreviewStoredFileName = previewStorageResult.StoredFileName,
                        PreviewRelativePath = previewStorageResult.RelativePath,
                        PreviewContentType = "image/jpeg",
                        PreviewSizeBytes = previewStorageResult.SizeBytes,
                        IsDownloadAllowed = false,
                        UploadedAt = DateTime.UtcNow
                    };

                    context.DigitalFiles.Add(digitalFile);
                    await context.SaveChangesAsync();

                    fileResult.Success = true;
                    batchResult.Results.Add(fileResult);
                    batchResult.SuccessCount++;
                }
                catch (DbUpdateException)
                {
                    DeleteIfExists(savedOriginalPath, originalFileStorageService);
                    DeleteIfExists(savedPreviewPath, previewImageStorageService);

                    fileResult.Success = false;
                    fileResult.Error = "Database error occurred while saving the file.";
                    batchResult.Results.Add(fileResult);
                    batchResult.FailedCount++;
                }
                catch (Exception ex)
                {
                    DeleteIfExists(savedOriginalPath, originalFileStorageService);
                    DeleteIfExists(savedPreviewPath, previewImageStorageService);

                    fileResult.Success = false;
                    fileResult.Error = $"Processing failed: {ex.Message}";
                    batchResult.Results.Add(fileResult);
                    batchResult.FailedCount++;
                }
            }

            if (batchResult.SuccessCount > 0)
            {
                bool itemHasFiles = await context.DigitalFiles.AnyAsync(df => df.ItemId == item.Id);
                if (itemHasFiles && item.Status != ItemStatus.Digitized)
                {
                    item.Status = ItemStatus.Digitized;
                    await context.SaveChangesAsync();
                }
            }

            return batchResult;
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

            if (!originalFileStorageService.Exists(digitalFile.OriginalRelativePath))
            {
                return null;
            }

            await using Stream stream = originalFileStorageService.OpenRead(digitalFile.OriginalRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.OriginalContentType,
                digitalFile.OriginalFileName
            );
        }

        public async Task<(byte[] Content, string ContentType, string DownloadName)?> DownloadPreviewAsync(int id, bool enforceUserAccess)
        {
            DigitalFile? digitalFile = await GetAccessibleDigitalFileAsync(id, enforceUserAccess);
            if (digitalFile is null)
            {
                return null;
            }

            if (!previewImageStorageService.Exists(digitalFile.PreviewRelativePath))
            {
                return null;
            }

            string previewDownloadName = Path.GetFileNameWithoutExtension(digitalFile.OriginalFileName) + ".jpg";

            await using Stream stream = previewImageStorageService.OpenRead(digitalFile.PreviewRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.PreviewContentType,
                previewDownloadName
            );
        }

        public async Task<(byte[] Content, string ContentType)?> GetPreviewImageAsync(int id, bool enforceUserAccess)
        {
            DigitalFile? digitalFile = await GetAccessibleDigitalFileAsync(id, enforceUserAccess);
            if (digitalFile is null)
            {
                return null;
            }

            if (!previewImageStorageService.Exists(digitalFile.PreviewRelativePath))
            {
                return null;
            }

            await using Stream stream = previewImageStorageService.OpenRead(digitalFile.PreviewRelativePath);
            await using MemoryStream output = new MemoryStream();
            await stream.CopyToAsync(output);

            return (
                output.ToArray(),
                digitalFile.PreviewContentType
            );
        }

        public async Task<DigitalFilePreviewViewModel?> GetPreviewPageAsync(
            int id,
            bool enforceUserAccess,
            bool canDownloadOriginal,
            bool canDownloadPreview,
            string backToItemDetailsArea)
        {
            DigitalFile? currentFile = await GetAccessibleDigitalFileAsync(id, enforceUserAccess);
            if (currentFile is null)
            {
                return null;
            }

            IQueryable<DigitalFile> query = context.DigitalFiles
                .AsNoTracking()
                .Where(df => df.ItemId == currentFile.ItemId);

            if (enforceUserAccess)
            {
                query = query.Where(df => df.IsDownloadAllowed);
            }

            List<DigitalFile> files = await query
                .OrderBy(df => df.UploadedAt)
                .ThenBy(df => df.Id)
                .ToListAsync();

            int index = files.FindIndex(df => df.Id == id);
            if (index < 0)
            {
                return null;
            }

            string previewImageUrl = backToItemDetailsArea == "Admin"
                ? $"/Admin/DigitalFiles/PreviewImage/{currentFile.Id}"
                : $"/User/DigitalFiles/PreviewImage/{currentFile.Id}";

            return new DigitalFilePreviewViewModel
            {
                Id = currentFile.Id,
                ItemId = currentFile.ItemId,
                OriginalFileName = currentFile.OriginalFileName,
                PreviewImageUrl = previewImageUrl,
                Position = index + 1,
                TotalCount = files.Count,
                PreviousFileId = index > 0 ? files[index - 1].Id : null,
                NextFileId = index < files.Count - 1 ? files[index + 1].Id : null,
                CanDownloadOriginal = canDownloadOriginal,
                CanDownloadPreview = canDownloadPreview,
                BackToItemDetailsArea = backToItemDetailsArea
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

            if (originalFileStorageService.Exists(digitalFile.OriginalRelativePath))
            {
                originalFileStorageService.Delete(digitalFile.OriginalRelativePath);
            }

            if (previewImageStorageService.Exists(digitalFile.PreviewRelativePath))
            {
                previewImageStorageService.Delete(digitalFile.PreviewRelativePath);
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

        private async Task<DigitalFile?> GetAccessibleDigitalFileAsync(int id, bool enforceUserAccess)
        {
            IQueryable<DigitalFile> query = context.DigitalFiles
                .AsNoTracking()
                .Where(df => df.Id == id);

            if (enforceUserAccess)
            {
                query = query.Where(df => df.IsDownloadAllowed);
            }

            return await query.FirstOrDefaultAsync();
        }

        private string? ValidateUploadFile(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return "File is empty.";
            }

            if (file.Length > options.MaxTiffUploadSizeBytes)
            {
                return $"File is too large. Max allowed is {options.MaxTiffUploadSizeBytes:N0} bytes.";
            }

            string extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExt.Contains(extension))
            {
                return "Only .tif / .tiff files are allowed.";
            }

            return null;
        }

        private static string ComputeSha256Hex(Stream stream)
        {
            stream.Position = 0;

            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(stream);

            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static void DeleteIfExists(string? relativePath, IOriginalFileStorageService storageService)
        {
            if (!string.IsNullOrWhiteSpace(relativePath) && storageService.Exists(relativePath))
            {
                storageService.Delete(relativePath);
            }
        }

        private static void DeleteIfExists(string? relativePath, IPreviewImageStorageService storageService)
        {
            if (!string.IsNullOrWhiteSpace(relativePath) && storageService.Exists(relativePath))
            {
                storageService.Delete(relativePath);
            }
        }
    }
}