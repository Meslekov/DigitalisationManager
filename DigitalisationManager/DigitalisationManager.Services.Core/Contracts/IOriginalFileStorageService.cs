namespace DigitalisationManager.Services.Core.Contracts
{
    public interface IOriginalFileStorageService
    {
        Task<(string StoredFileName, string RelativePath, long SizeBytes)> SaveAsync(
            int fundId,
            int itemId,
            string extension,
            Stream content);

        bool Exists(string relativePath);

        Stream OpenRead(string relativePath);

        void Delete(string relativePath);
    }
}