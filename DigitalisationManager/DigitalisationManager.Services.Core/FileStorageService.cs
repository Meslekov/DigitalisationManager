namespace DigitalisationManager.Services.Core
{
    using Microsoft.Extensions.Options;
   
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Services.Core.Options;

    public class FileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions options;

        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            this.options = options.Value;
        }
        public async Task SaveAsync(string relativePath, Stream content)
        {
            string abs = GetAbsolutePath(relativePath);
            string? dir = Path.GetDirectoryName(abs);

            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            content.Position = 0;
            using FileStream fs = new FileStream(abs, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await content.CopyToAsync(fs);
        }


        public Stream OpenRead(string relativePath)
        {
            string abs = GetAbsolutePath(relativePath);
            return new FileStream(abs, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public bool Exists(string relativePath)
            => File.Exists(GetAbsolutePath(relativePath));
        public void Delete(string relativePath)
        {
            string abs = GetAbsolutePath(relativePath);
            if (File.Exists(abs))
            {
                File.Delete(abs);
            }
        }
        public string GetAbsolutePath(string relativePath)
        {
            // Store relative paths in this way: Funds/1/Items/2/Tiffs/{guid}.tif
            // The physical path is: {RootFolder}/Funds/...
            string safeRelative = relativePath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);

            return Path.Combine(options.RootFolder, safeRelative);
        }
    }
}
