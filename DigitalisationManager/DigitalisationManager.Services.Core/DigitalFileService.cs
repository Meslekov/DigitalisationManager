namespace DigitalisationManager.Services.Core
{
    using System.Security.Cryptography;

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
        private static readonly HashSet<string> AllowedExt 
            = new(StringComparer.OrdinalIgnoreCase) 
                 { ".tif", ".tiff" };

        private readonly DigitalisationManagerDbContext context;
        private readonly IFileStorageService filestorage;
        private readonly FileStorageOptions options;

        public DigitalFileService(
            DigitalisationManagerDbContext context,
            IFileStorageService filestorage,
            IOptions<FileStorageOptions> options)
        {
            this.context = context;
            this.filestorage = filestorage;
            this.options = options.Value;
        }
        public async Task<(bool Success, string? Error)> UploadTiffAsync(
            int itemId,
            Stream contentStream,
            string originalFileName,
            long sizeBytes)
        {
            if (contentStream is null || sizeBytes == 0)
                return (false, "Please choose a file.");

            if (sizeBytes > options.MaxTiffUploadSizeBytes)
                return (false, $"File is too large. Max allowed is {options.MaxTiffUploadSizeBytes:N0} bytes.");

            string ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExt.Contains(ext))
                return (false, "Only .tif / .tiff files are allowed.");

            var item = await context.Items
                .Include(i => i.DigitalFiles)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item is null)
                return (false, "Item not found.");

            string storedName = $"{Guid.NewGuid():N}.tif";
            string relativePath = $"Funds/{item.FundId}/Items/{item.Id}/Tiffs/{storedName}";

            using var ms = new MemoryStream();
            await contentStream.CopyToAsync(ms);
            ms.Position = 0;

            string sha256 = ComputeSha256Hex(ms);
            ms.Position = 0;

            await filestorage.SaveAsync(relativePath, ms);

            var entity = new DigitalFile
            {
                ItemId = item.Id,
                OriginalFileName = Path.GetFileName(originalFileName),
                StoredFileName = storedName,
                RelativePath = relativePath,
                SizeBytes = sizeBytes,
                UploadedAt = DateTime.UtcNow,
                ChecksumSha256 = sha256
            };

            context.DigitalFiles.Add(entity);

            if (!item.DigitalFiles.Any())
                item.Status = ItemStatus.Digitized;

            await context.SaveChangesAsync();

            return (true, null);
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
                                SizeBytes = df.SizeBytes,
                                UploadedAt = df.UploadedAt,
                                ChecksumSha256 = df.ChecksumSha256
                            })
                            .ToListAsync();
        }

        public async Task<(
            bool Found,
            string? OriginalFileName,
            Stream? ContentStream)> OpenDownloadStreamAsync(int digitalFileId)
        {
            var df = await context.DigitalFiles
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Id == digitalFileId);

            if (df is null)
                return (false, null, null);

            if (!filestorage.Exists(df.RelativePath))
                return (false, null, null);

            Stream s = filestorage.OpenRead(df.RelativePath);
            return (true, df.OriginalFileName, s);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int digitalFileId)
        {
            var df = await context.DigitalFiles.FirstOrDefaultAsync(x => x.Id == digitalFileId);
            if (df is null) return (false, "File not found.");

            filestorage.Delete(df.RelativePath);

            context.DigitalFiles.Remove(df);
            await context.SaveChangesAsync();

            return (true, null);
        }

        private static string ComputeSha256Hex(Stream s)
        {
            s.Position = 0;
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(s);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
