namespace DigitalisationManager.Services.Core.Contracts
{
    public interface ITiffConversionService
    {
        Task<byte[]> ConvertFirstPageToJpegAsync(Stream tiffStream);
    }
}