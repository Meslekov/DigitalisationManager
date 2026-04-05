namespace DigitalisationManager.Services.Core.Contracts
{
    using Microsoft.AspNetCore.Http;

    using DigitalisationManager.Web.ViewModels.DigitalFile;

    public interface IDigitalFileService
    {
        Task<IReadOnlyList<DigitalFileListViewModel>> ListByItemAsync(int itemId);

        Task<BatchDigitalFileUploadResultViewModel> UploadAsync(int itemId, IEnumerable<IFormFile> files);

        Task<(byte[] Content, string ContentType, string DownloadName)?> DownloadOriginalAsync(int id);

        Task<(byte[] Content, string ContentType, string DownloadName)?> DownloadPreviewAsync(int id);

        Task<(byte[] Content, string ContentType)?> GetPreviewImageAsync(int id);

        Task<DigitalFilePreviewViewModel?> GetPreviewPageAsync(int id, bool canDownloadOriginal, bool canDownloadPreview);

        Task<(bool Success, string? Error)> DeleteAsync(int id);

        Task<(bool Success, string? Error)> SetDownloadAllowedAsync(int id, bool isAllowed);
    }
}