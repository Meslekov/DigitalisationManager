namespace DigitalisationManager.Services.Core.Contracts
{
    using Microsoft.AspNetCore.Http;

    using DigitalisationManager.Web.ViewModels.DigitalFile;

    public interface IDigitalFileService
    {
        Task<(bool Success, string? Error)> UploadTiffAsync(
            int itemId,
            Stream contentStream,
            string originalFileName,
            long sizeBytes);
        Task<IReadOnlyList<DigitalFileListViewModel>> ListByItemAsync(int itemId);

        Task<(bool Found, string? OriginalFileName, Stream? ContentStream)> OpenDownloadStreamAsync(int digitalFileId);

        Task<(bool Success, string? Error)> DeleteAsync(int digitalFileId);
    }
}
