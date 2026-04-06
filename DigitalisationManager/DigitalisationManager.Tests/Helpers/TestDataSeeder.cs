namespace DigitalisationManager.Tests.Helpers;

public static class TestDataSeeder
{
    public static async Task SeedBasicAsync(DigitalisationManagerDbContext context)
    {
        if (await context.Funds.AnyAsync() || await context.Categories.AnyAsync())
        {
            return;
        }

        context.Funds.AddRange(
            new Fund
            {
                Id = 1,
                FundType = FundType.ArchiveFund,
                Code = "AF-001",
                Title = "Archive Fund 001",
                Description = "Test fund 1",
                CreatedAt = DateTime.UtcNow
            },
            new Fund
            {
                Id = 2,
                FundType = FundType.PhotoFund,
                Code = "PF-001",
                Title = "Photo Fund 001",
                Description = "Test fund 2",
                CreatedAt = DateTime.UtcNow
            });

        context.Categories.AddRange(
            new Category
            {
                Id = 1,
                Name = "Administrative Records",
                Description = "Active category",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 2,
                Name = "Inactive Category",
                Description = "Inactive category",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 3,
                Name = "Correspondence",
                Description = "Second active category",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();
    }

    public static async Task<Item> AddItemAsync(
        DigitalisationManagerDbContext context,
        int id = 1,
        int fundId = 1,
        int categoryId = 1,
        string inventoryNumber = "INV-001",
        string? description = "Initial description",
        string? documentDateText = "1901",
        ItemStatus status = ItemStatus.New)
    {
        var item = new Item
        {
            Id = id,
            FundId = fundId,
            CategoryId = categoryId,
            InventoryNumber = inventoryNumber,
            Description = description,
            DocumentDateText = documentDateText,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        context.Items.Add(item);
        await context.SaveChangesAsync();

        return item;
    }

    public static async Task<DigitalFile> AddDigitalFileAsync(
        DigitalisationManagerDbContext context,
        int id = 1,
        int itemId = 1,
        string originalFileName = "scan-001.tif")
    {
        var file = new DigitalFile
        {
            Id = id,
            ItemId = itemId,
            OriginalFileName = originalFileName,
            OriginalStoredFileName = $"stored-{id}.tif",
            OriginalRelativePath = $"originals/{itemId}/stored-{id}.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 1024,
            OriginalChecksumSha256 = new string('a', 64),
            PreviewStoredFileName = $"preview-{id}.jpg",
            PreviewRelativePath = $"previews/{itemId}/preview-{id}.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 512,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        context.DigitalFiles.Add(file);
        await context.SaveChangesAsync();

        return file;
    }
}