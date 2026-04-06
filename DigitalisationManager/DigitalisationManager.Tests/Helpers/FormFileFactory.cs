namespace DigitalisationManager.Tests.Helpers;

using Microsoft.AspNetCore.Http.Internal;

public static class FormFileFactory
{
    public static IFormFile Create(
        string fileName,
        string content,
        string contentType = "image/tiff")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    public static IFormFile Create(
        string fileName,
        byte[] bytes,
        string contentType = "image/tiff")
    {
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}