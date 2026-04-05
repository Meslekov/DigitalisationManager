namespace DigitalisationManager.Services.Core
{
    using ImageMagick;

    using DigitalisationManager.Services.Core.Contracts;

    public class TiffConversionService : ITiffConversionService
    {
        public async Task<byte[]> ConvertFirstPageToJpegAsync(Stream tiffStream)
        {
            if (tiffStream is null || !tiffStream.CanRead)
            {
                throw new InvalidOperationException("Invalid TIFF stream.");
            }

            tiffStream.Position = 0;

            using MemoryStream inputBuffer = new MemoryStream();
            await tiffStream.CopyToAsync(inputBuffer);

            inputBuffer.Position = 0;

            MagickReadSettings settings = new MagickReadSettings
            {
                FrameIndex = 0,
                FrameCount = 1
            };

            using MagickImage image = new MagickImage(inputBuffer, settings);

            image.Format = MagickFormat.Jpeg;
            image.Quality = 85;

            using MemoryStream output = new MemoryStream();
            await image.WriteAsync(output);

            return output.ToArray();
        }
    }
}