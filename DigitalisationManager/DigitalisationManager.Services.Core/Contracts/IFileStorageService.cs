namespace DigitalisationManager.Services.Core.Contracts
{
    public interface IFileStorageService
    {
        Task SaveAsync(string relativePath, Stream content);

        Stream OpenRead(string relativePath);

        bool Exists(string relativePath);

        void Delete(string relativePath);

        string GetAbsolutePath(string relativePath);
    }
}
