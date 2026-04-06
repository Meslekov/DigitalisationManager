namespace DigitalisationManager.Tests.Fakes;

using DigitalisationManager.Services.Core.Contracts;

public class FakeOriginalFileStorageService : IOriginalFileStorageService
{
    public Dictionary<string, byte[]> Files { get; } = new();

    public bool SaveCalled { get; private set; }
    public bool DeleteCalled { get; private set; }
    public string? LastSavedPath { get; private set; }
    public string? LastDeletedPath { get; private set; }

    public bool ShouldThrowOnSave { get; set; }
    public bool ShouldThrowOnDelete { get; set; }

    public Task<(string StoredFileName, string RelativePath, long SizeBytes)> SaveAsync(
        int fundId,
        int itemId,
        string extension,
        Stream content)
    {
        if (ShouldThrowOnSave)
        {
            throw new InvalidOperationException("Simulated original file save failure.");
        }

        using MemoryStream memory = new MemoryStream();
        content.Position = 0;
        content.CopyTo(memory);

        string normalizedExtension = extension.StartsWith(".")
            ? extension.ToLowerInvariant()
            : "." + extension.ToLowerInvariant();

        string storedFileName = $"{Guid.NewGuid():N}{normalizedExtension}";
        string relativePath = $"Funds/{fundId}/Items/{itemId}/{storedFileName}";
        byte[] bytes = memory.ToArray();

        SaveCalled = true;
        LastSavedPath = relativePath;
        Files[relativePath] = bytes;

        return Task.FromResult((storedFileName, relativePath, (long)bytes.Length));
    }

    public bool Exists(string relativePath)
    {
        return Files.ContainsKey(relativePath);
    }

    public Stream OpenRead(string relativePath)
    {
        if (!Files.TryGetValue(relativePath, out byte[]? bytes))
        {
            throw new FileNotFoundException("Fake original file not found.", relativePath);
        }

        return new MemoryStream(bytes);
    }

    public void Delete(string relativePath)
    {
        if (ShouldThrowOnDelete)
        {
            throw new InvalidOperationException("Simulated original file delete failure.");
        }

        DeleteCalled = true;
        LastDeletedPath = relativePath;
        Files.Remove(relativePath);
    }
}