namespace DigitalisationManager.Tests.Fakes;

using DigitalisationManager.Services.Core.Contracts;

public class FakeTiffConversionService : ITiffConversionService
{
    public bool ConvertCalled { get; private set; }
    public bool ShouldThrowOnConvert { get; set; }

    public byte[] ResultBytes { get; set; } = new byte[] { 1, 2, 3, 4, 5 };

    public Task<byte[]> ConvertFirstPageToJpegAsync(Stream tiffStream)
    {
        if (ShouldThrowOnConvert)
        {
            throw new InvalidOperationException("Simulated TIFF conversion failure.");
        }

        ConvertCalled = true;
        return Task.FromResult(ResultBytes);
    }
}