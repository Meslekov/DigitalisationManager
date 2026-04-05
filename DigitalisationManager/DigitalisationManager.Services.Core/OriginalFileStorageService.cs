namespace DigitalisationManager.Services.Core
{
    using Microsoft.Extensions.Options;

    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Services.Core.Options;

    public class OriginalFileStorageService : IOriginalFileStorageService
    {
        private readonly string rootPath;

        public OriginalFileStorageService(IOptions<FileStorageOptions> options)
        {
            string basePath = options.Value.RootFolder;
            rootPath = Path.Combine(basePath, "Originals");
        }

        public async Task<(string StoredFileName, string RelativePath, long SizeBytes)> SaveAsync(
            int fundId,
            int itemId,
            string extension,
            Stream content)
        {
            string normalizedExtension = NormalizeExtension(extension);
            string storedFileName = $"{Guid.NewGuid():N}{normalizedExtension}";
            string relativePath = Path.Combine("Funds", fundId.ToString(), "Items", itemId.ToString(), storedFileName);
            string fullPath = GetFullPath(relativePath);

            string? directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            content.Position = 0;

            await using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fileStream);

            long sizeBytes = new FileInfo(fullPath).Length;

            return (storedFileName, relativePath.Replace('\\', '/'), sizeBytes);
        }

        public bool Exists(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);
            return File.Exists(fullPath);
        }

        public Stream OpenRead(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void Delete(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string GetFullPath(string relativePath)
            => Path.Combine(rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));

        private static string NormalizeExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return ".tif";
            }

            return extension.StartsWith('.')
                ? extension.ToLowerInvariant()
                : $".{extension.ToLowerInvariant()}";
        }
    }
}