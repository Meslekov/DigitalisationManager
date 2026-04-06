using DigitalisationManager.Services.Core;
using DigitalisationManager.Services.Core.Options;
using DigitalisationManager.Tests.Fakes;
using DigitalisationManager.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace DigitalisationManager.Tests.Services;

[TestFixture]
public class DigitalFileServiceTests
{
    private DigitalisationManagerDbContext _context = null!;
    private FakeOriginalFileStorageService _originalStorage = null!;
    private FakePreviewImageStorageService _previewStorage = null!;
    private FakeTiffConversionService _tiffConversion = null!;
    private DigitalFileService _service = null!;

    [SetUp]
    public async Task Setup()
    {
        _context = TestDbContextFactory.Create();
        await TestDataSeeder.SeedBasicAsync(_context);

        _originalStorage = new FakeOriginalFileStorageService();
        _previewStorage = new FakePreviewImageStorageService();
        _tiffConversion = new FakeTiffConversionService();

        IOptions<FileStorageOptions> options = Options.Create(new FileStorageOptions
        {
            RootFolder = "TestStorage",
            MaxTiffUploadSizeBytes = 10
        });

        _service = new DigitalFileService(
            _context,
            _originalStorage,
            _previewStorage,
            _tiffConversion,
            options);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task UploadAsync_ShouldReturnFailure_WhenItemDoesNotExist()
    {
        var file = FormFileFactory.Create("scan-001.tif", new byte[] { 1, 2, 3 }, "image/tiff");

        var result = await _service.UploadAsync(999, new[] { file });

        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("Item 999"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Item not found."));
    }

    [Test]
    public async Task UploadAsync_ShouldReturnFailure_WhenNoFilesAreSelected()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 50);

        var result = await _service.UploadAsync(item.Id, Array.Empty<IFormFile>());

        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("No files selected"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Please choose at least one TIFF file."));
    }

    [Test]
    public async Task UploadAsync_ShouldRejectFile_WhenExtensionIsInvalid()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 51);
        var file = FormFileFactory.Create("scan-001.jpg", new byte[] { 1, 2, 3 }, "image/jpeg");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-001.jpg"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Only .tif / .tiff files are allowed."));

        Assert.That(_tiffConversion.ConvertCalled, Is.False);
        Assert.That(_originalStorage.SaveCalled, Is.False);
        Assert.That(_previewStorage.SaveCalled, Is.False);
        Assert.That(_context.DigitalFiles.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task UploadAsync_ShouldRejectFile_WhenFileIsTooLarge()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 52);
        var file = FormFileFactory.Create("scan-001.tif", new byte[11], "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-001.tif"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("File is too large. Max allowed is 10 bytes."));

        Assert.That(_tiffConversion.ConvertCalled, Is.False);
        Assert.That(_originalStorage.SaveCalled, Is.False);
        Assert.That(_previewStorage.SaveCalled, Is.False);
        Assert.That(_context.DigitalFiles.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task UploadAsync_ShouldReturnFailure_WhenTiffConversionFails()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 53);
        var file = FormFileFactory.Create("scan-001.tif", new byte[] { 1, 2, 3, 4 }, "image/tiff");

        _tiffConversion.ShouldThrowOnConvert = true;

        var result = await _service.UploadAsync(item.Id, new[] { file });

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-001.tif"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Processing failed: Simulated TIFF conversion failure."));

        Assert.That(_tiffConversion.ConvertCalled, Is.False);
        Assert.That(_originalStorage.SaveCalled, Is.False);
        Assert.That(_previewStorage.SaveCalled, Is.False);
        Assert.That(_originalStorage.Files, Is.Empty);
        Assert.That(_previewStorage.Files, Is.Empty);
        Assert.That(_context.DigitalFiles.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task UploadAsync_ShouldSaveOriginalAndPreviewAndCreateDatabaseRecord_WhenFileIsValid()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 60,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-001",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("scan-001.tif", new byte[] { 1, 2, 3, 4, 5 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var saved = await _context.DigitalFiles.SingleAsync(df => df.ItemId == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(result.FailedCount, Is.EqualTo(0));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-001.tif"));
        Assert.That(result.Results[0].Success, Is.True);
        Assert.That(result.Results[0].Error, Is.Null);

        Assert.That(_tiffConversion.ConvertCalled, Is.True);
        Assert.That(_originalStorage.SaveCalled, Is.True);
        Assert.That(_previewStorage.SaveCalled, Is.True);

        Assert.That(_originalStorage.Files.Count, Is.EqualTo(1));
        Assert.That(_previewStorage.Files.Count, Is.EqualTo(1));

        Assert.That(saved.OriginalFileName, Is.EqualTo("scan-001.tif"));
        Assert.That(saved.OriginalContentType, Is.EqualTo("image/tiff"));
        Assert.That(saved.PreviewContentType, Is.EqualTo("image/jpeg"));
        Assert.That(saved.IsDownloadAllowed, Is.False);

        Assert.That(saved.OriginalStoredFileName, Does.EndWith(".tif"));
        Assert.That(saved.PreviewStoredFileName, Does.EndWith(".jpg"));
        Assert.That(saved.OriginalRelativePath, Is.Not.Null.And.Not.Empty);
        Assert.That(saved.PreviewRelativePath, Is.Not.Null.And.Not.Empty);
        Assert.That(saved.OriginalChecksumSha256, Has.Length.EqualTo(64));

        Assert.That(_originalStorage.Exists(saved.OriginalRelativePath), Is.True);
        Assert.That(_previewStorage.Exists(saved.PreviewRelativePath), Is.True);
    }

    [Test]
    public async Task UploadAsync_ShouldNormalizeTiffExtensionToDotTiff_WhenInputFileExtensionIsTiff()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 61,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-002",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("scan-002.tiff", new byte[] { 10, 20, 30, 40 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var saved = await _context.DigitalFiles.SingleAsync(df => df.ItemId == item.Id);

        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(saved.OriginalFileName, Is.EqualTo("scan-002.tiff"));
        Assert.That(saved.OriginalStoredFileName, Does.EndWith(".tiff"));
        Assert.That(saved.OriginalRelativePath, Does.EndWith(".tiff"));
    }

    [Test]
    public async Task UploadAsync_ShouldSetItemStatusToDigitized_WhenAtLeastOneFileSucceeds()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 62,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-003",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("scan-003.tif", new byte[] { 9, 8, 7, 6 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.Digitized));
    }

    [Test]
    public async Task UploadAsync_ShouldKeepItemStatusDigitized_WhenItemIsAlreadyDigitized()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 63,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-004",
            status: ItemStatus.Digitized);

        var file = FormFileFactory.Create("scan-004.tif", new byte[] { 1, 1, 1, 1 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.Digitized));
    }

    [Test]
    public async Task UploadAsync_ShouldCreateMultipleRecords_WhenMultipleValidFilesAreUploaded()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 64,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-005",
            status: ItemStatus.New);

        var file1 = FormFileFactory.Create("scan-a.tif", new byte[] { 1, 2, 3 }, "image/tiff");
        var file2 = FormFileFactory.Create("scan-b.tiff", new byte[] { 4, 5, 6 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file1, file2 });

        var savedFiles = await _context.DigitalFiles
            .Where(df => df.ItemId == item.Id)
            .OrderBy(df => df.OriginalFileName)
            .ToListAsync();

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.SuccessCount, Is.EqualTo(2));
        Assert.That(result.FailedCount, Is.EqualTo(0));
        Assert.That(result.Results.Count, Is.EqualTo(2));
        Assert.That(result.Results.All(r => r.Success), Is.True);

        Assert.That(savedFiles.Count, Is.EqualTo(2));
        Assert.That(savedFiles[0].OriginalFileName, Is.EqualTo("scan-a.tif"));
        Assert.That(savedFiles[1].OriginalFileName, Is.EqualTo("scan-b.tiff"));

        Assert.That(_originalStorage.Files.Count, Is.EqualTo(2));
        Assert.That(_previewStorage.Files.Count, Is.EqualTo(2));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.Digitized));
    }

    [Test]
    public async Task UploadAsync_ShouldContinueProcessing_WhenOneFileFailsAndAnotherSucceeds()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 65,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-006",
            status: ItemStatus.New);

        var invalidFile = FormFileFactory.Create("invalid.jpg", new byte[] { 1, 2, 3 }, "image/jpeg");
        var validFile = FormFileFactory.Create("valid.tif", new byte[] { 4, 5, 6, 7 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { invalidFile, validFile });

        var savedFiles = await _context.DigitalFiles
            .Where(df => df.ItemId == item.Id)
            .ToListAsync();

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(2));

        Assert.That(result.Results.Any(r =>
            r.FileName == "invalid.jpg" &&
            r.Success == false &&
            r.Error == "Only .tif / .tiff files are allowed."), Is.True);

        Assert.That(result.Results.Any(r =>
            r.FileName == "valid.tif" &&
            r.Success), Is.True);

        Assert.That(savedFiles.Count, Is.EqualTo(1));
        Assert.That(savedFiles[0].OriginalFileName, Is.EqualTo("valid.tif"));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.Digitized));
    }

    [Test]
    public async Task UploadAsync_ShouldNotSaveAnything_WhenOriginalFileSaveFails()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 70,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-007",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("scan-savefail.tif", new byte[] { 1, 2, 3, 4 }, "image/tiff");

        _originalStorage.ShouldThrowOnSave = true;

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var digitalFilesCount = await _context.DigitalFiles.CountAsync(df => df.ItemId == item.Id);
        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-savefail.tif"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Processing failed: Simulated original file save failure."));

        Assert.That(_tiffConversion.ConvertCalled, Is.True);
        Assert.That(_originalStorage.SaveCalled, Is.False);
        Assert.That(_previewStorage.SaveCalled, Is.False);

        Assert.That(_originalStorage.Files, Is.Empty);
        Assert.That(_previewStorage.Files, Is.Empty);
        Assert.That(digitalFilesCount, Is.EqualTo(0));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.New));
    }

    [Test]
    public async Task UploadAsync_ShouldDeleteSavedOriginal_WhenPreviewSaveFails()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 71,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-008",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("scan-previewfail.tif", new byte[] { 5, 6, 7, 8 }, "image/tiff");

        _previewStorage.ShouldThrowOnSave = true;

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var digitalFilesCount = await _context.DigitalFiles.CountAsync(df => df.ItemId == item.Id);
        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.Results.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].FileName, Is.EqualTo("scan-previewfail.tif"));
        Assert.That(result.Results[0].Success, Is.False);
        Assert.That(result.Results[0].Error, Is.EqualTo("Processing failed: Simulated preview save failure."));

        Assert.That(_tiffConversion.ConvertCalled, Is.True);
        Assert.That(_originalStorage.SaveCalled, Is.True);
        Assert.That(_previewStorage.SaveCalled, Is.False);

        Assert.That(_originalStorage.DeleteCalled, Is.True);
        Assert.That(_originalStorage.LastDeletedPath, Is.EqualTo(_originalStorage.LastSavedPath));

        Assert.That(_originalStorage.Files, Is.Empty);
        Assert.That(_previewStorage.Files, Is.Empty);
        Assert.That(digitalFilesCount, Is.EqualTo(0));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.New));
    }

    [Test]
    public async Task UploadAsync_ShouldKeepItemStatusUnchanged_WhenAllFilesFail()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 72,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-009",
            status: ItemStatus.InProgress);

        var invalidExtensionFile = FormFileFactory.Create("bad.jpg", new byte[] { 1, 2, 3 }, "image/jpeg");
        var tooLargeFile = FormFileFactory.Create("huge.tif", new byte[11], "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { invalidExtensionFile, tooLargeFile });

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);
        var digitalFilesCount = await _context.DigitalFiles.CountAsync(df => df.ItemId == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.SuccessCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(2));

        Assert.That(digitalFilesCount, Is.EqualTo(0));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.InProgress));
        Assert.That(_originalStorage.Files, Is.Empty);
        Assert.That(_previewStorage.Files, Is.Empty);
    }

    [Test]
    public async Task UploadAsync_ShouldKeepSuccessfulFiles_WhenAnotherFileFailsLater()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 73,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-010",
            status: ItemStatus.New);

        var validFile = FormFileFactory.Create("good.tif", new byte[] { 10, 20, 30, 40 }, "image/tiff");
        var invalidFile = FormFileFactory.Create("bad.jpg", new byte[] { 50, 60, 70 }, "image/jpeg");

        var result = await _service.UploadAsync(item.Id, new[] { validFile, invalidFile });

        var digitalFiles = await _context.DigitalFiles
            .Where(df => df.ItemId == item.Id)
            .ToListAsync();

        var updatedItem = await _context.Items.SingleAsync(i => i.Id == item.Id);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(result.FailedCount, Is.EqualTo(1));

        Assert.That(digitalFiles.Count, Is.EqualTo(1));
        Assert.That(digitalFiles[0].OriginalFileName, Is.EqualTo("good.tif"));

        Assert.That(_originalStorage.Files.Count, Is.EqualTo(1));
        Assert.That(_previewStorage.Files.Count, Is.EqualTo(1));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.Digitized));
    }

    [Test]
    public async Task UploadAsync_ShouldUseExactOriginalFileNameInDatabase_WhenUploadSucceeds()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 74,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-DF-011",
            status: ItemStatus.New);

        var file = FormFileFactory.Create("My Scan Final.TIF", new byte[] { 1, 3, 5, 7 }, "image/tiff");

        var result = await _service.UploadAsync(item.Id, new[] { file });

        var saved = await _context.DigitalFiles.SingleAsync(df => df.ItemId == item.Id);

        Assert.That(result.SuccessCount, Is.EqualTo(1));
        Assert.That(saved.OriginalFileName, Is.EqualTo("My Scan Final.TIF"));
    }

    [Test]
    public async Task DownloadOriginalAsync_ShouldReturnNull_WhenFileDoesNotExistInDatabase()
    {
        var result = await _service.DownloadOriginalAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadOriginalAsync_ShouldReturnNull_WhenStoredOriginalFileIsMissing()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 80);

        var digitalFile = new DigitalFile
        {
            Id = 801,
            ItemId = item.Id,
            OriginalFileName = "scan-original.tif",
            OriginalStoredFileName = "stored-original.tif",
            OriginalRelativePath = "Funds/1/Items/80/stored-original.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 4,
            OriginalChecksumSha256 = new string('a', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = "Funds/1/Items/80/preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 3,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DownloadOriginalAsync(digitalFile.Id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadOriginalAsync_ShouldReturnContent_WhenStoredOriginalExists()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 81);

        var originalBytes = new byte[] { 1, 2, 3, 4, 5 };
        var originalPath = "Funds/1/Items/81/stored-original.tif";
        _originalStorage.Files[originalPath] = originalBytes;

        var digitalFile = new DigitalFile
        {
            Id = 802,
            ItemId = item.Id,
            OriginalFileName = "scan-original.tif",
            OriginalStoredFileName = "stored-original.tif",
            OriginalRelativePath = originalPath,
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = originalBytes.Length,
            OriginalChecksumSha256 = new string('b', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = "Funds/1/Items/81/preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 3,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DownloadOriginalAsync(digitalFile.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.Content, Is.EqualTo(originalBytes));
        Assert.That(result.Value.ContentType, Is.EqualTo("image/tiff"));
        Assert.That(result.Value.DownloadName, Is.EqualTo("scan-original.tif"));
    }

    [Test]
    public async Task DownloadPreviewAsync_ShouldReturnNull_WhenUserAccessIsEnforcedAndDownloadIsNotAllowed()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 82);

        var digitalFile = new DigitalFile
        {
            Id = 803,
            ItemId = item.Id,
            OriginalFileName = "scan-preview.tif",
            OriginalStoredFileName = "stored-preview.tif",
            OriginalRelativePath = "Funds/1/Items/82/stored-preview.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('c', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = "Funds/1/Items/82/preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DownloadPreviewAsync(digitalFile.Id, enforceUserAccess: true);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadPreviewAsync_ShouldReturnNull_WhenPreviewFileIsMissing()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 83);

        var digitalFile = new DigitalFile
        {
            Id = 804,
            ItemId = item.Id,
            OriginalFileName = "scan-preview-missing.tif",
            OriginalStoredFileName = "stored-preview-missing.tif",
            OriginalRelativePath = "Funds/1/Items/83/stored-preview-missing.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('d', 64),
            PreviewStoredFileName = "preview-missing.jpg",
            PreviewRelativePath = "Funds/1/Items/83/preview-missing.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DownloadPreviewAsync(digitalFile.Id, enforceUserAccess: true);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadPreviewAsync_ShouldReturnPreview_WhenAccessibleAndStoredFileExists()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 84);

        var previewBytes = new byte[] { 9, 8, 7 };
        var previewPath = "Funds/1/Items/84/preview.jpg";
        _previewStorage.Files[previewPath] = previewBytes;

        var digitalFile = new DigitalFile
        {
            Id = 805,
            ItemId = item.Id,
            OriginalFileName = "scan-preview-success.tif",
            OriginalStoredFileName = "stored-preview-success.tif",
            OriginalRelativePath = "Funds/1/Items/84/stored-preview-success.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('e', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = previewPath,
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = previewBytes.Length,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DownloadPreviewAsync(digitalFile.Id, enforceUserAccess: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.Content, Is.EqualTo(previewBytes));
        Assert.That(result.Value.ContentType, Is.EqualTo("image/jpeg"));
        Assert.That(result.Value.DownloadName, Is.EqualTo("scan-preview-success.jpg"));
    }

    [Test]
    public async Task GetPreviewImageAsync_ShouldReturnNull_WhenUserAccessIsEnforcedAndDownloadIsNotAllowed()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 85);

        var digitalFile = new DigitalFile
        {
            Id = 806,
            ItemId = item.Id,
            OriginalFileName = "scan-image.tif",
            OriginalStoredFileName = "stored-image.tif",
            OriginalRelativePath = "Funds/1/Items/85/stored-image.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('f', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = "Funds/1/Items/85/preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.GetPreviewImageAsync(digitalFile.Id, enforceUserAccess: true);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetPreviewImageAsync_ShouldReturnContent_WhenAccessibleAndStoredFileExists()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 86);

        var previewBytes = new byte[] { 6, 5, 4 };
        var previewPath = "Funds/1/Items/86/preview.jpg";
        _previewStorage.Files[previewPath] = previewBytes;

        var digitalFile = new DigitalFile
        {
            Id = 807,
            ItemId = item.Id,
            OriginalFileName = "scan-image-success.tif",
            OriginalStoredFileName = "stored-image-success.tif",
            OriginalRelativePath = "Funds/1/Items/86/stored-image-success.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('1', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = previewPath,
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = previewBytes.Length,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.GetPreviewImageAsync(digitalFile.Id, enforceUserAccess: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.Content, Is.EqualTo(previewBytes));
        Assert.That(result.Value.ContentType, Is.EqualTo("image/jpeg"));
    }

    [Test]
    public async Task GetPreviewPageAsync_ShouldReturnNull_WhenRequestedFileIsNotAccessible()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 87);

        var digitalFile = new DigitalFile
        {
            Id = 808,
            ItemId = item.Id,
            OriginalFileName = "scan-page.tif",
            OriginalStoredFileName = "stored-page.tif",
            OriginalRelativePath = "Funds/1/Items/87/stored-page.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('2', 64),
            PreviewStoredFileName = "preview.jpg",
            PreviewRelativePath = "Funds/1/Items/87/preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.GetPreviewPageAsync(
            digitalFile.Id,
            enforceUserAccess: true,
            canDownloadOriginal: false,
            canDownloadPreview: false,
            backToItemDetailsArea: "User");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetPreviewPageAsync_ShouldReturnNavigationData_ForAdminArea()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 88);

        var first = new DigitalFile
        {
            Id = 809,
            ItemId = item.Id,
            OriginalFileName = "a.tif",
            OriginalStoredFileName = "a-stored.tif",
            OriginalRelativePath = "Funds/1/Items/88/a-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('3', 64),
            PreviewStoredFileName = "a-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/88/a-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var second = new DigitalFile
        {
            Id = 810,
            ItemId = item.Id,
            OriginalFileName = "b.tif",
            OriginalStoredFileName = "b-stored.tif",
            OriginalRelativePath = "Funds/1/Items/88/b-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('4', 64),
            PreviewStoredFileName = "b-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/88/b-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        var third = new DigitalFile
        {
            Id = 811,
            ItemId = item.Id,
            OriginalFileName = "c.tif",
            OriginalStoredFileName = "c-stored.tif",
            OriginalRelativePath = "Funds/1/Items/88/c-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('5', 64),
            PreviewStoredFileName = "c-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/88/c-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.AddRange(first, second, third);
        await _context.SaveChangesAsync();

        var result = await _service.GetPreviewPageAsync(
            second.Id,
            enforceUserAccess: false,
            canDownloadOriginal: true,
            canDownloadPreview: true,
            backToItemDetailsArea: "Admin");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(second.Id));
        Assert.That(result.ItemId, Is.EqualTo(item.Id));
        Assert.That(result.Position, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(3));
        Assert.That(result.PreviousFileId, Is.EqualTo(first.Id));
        Assert.That(result.NextFileId, Is.EqualTo(third.Id));
        Assert.That(result.CanDownloadOriginal, Is.True);
        Assert.That(result.CanDownloadPreview, Is.True);
        Assert.That(result.BackToItemDetailsArea, Is.EqualTo("Admin"));
        Assert.That(result.PreviewImageUrl, Is.EqualTo($"/Admin/DigitalFiles/PreviewImage/{second.Id}"));
    }

    [Test]
    public async Task GetPreviewPageAsync_ShouldFilterNavigation_WhenUserAccessIsEnforced()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 89);

        var hidden = new DigitalFile
        {
            Id = 812,
            ItemId = item.Id,
            OriginalFileName = "hidden.tif",
            OriginalStoredFileName = "hidden-stored.tif",
            OriginalRelativePath = "Funds/1/Items/89/hidden-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('6', 64),
            PreviewStoredFileName = "hidden-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/89/hidden-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var visible1 = new DigitalFile
        {
            Id = 813,
            ItemId = item.Id,
            OriginalFileName = "visible1.tif",
            OriginalStoredFileName = "visible1-stored.tif",
            OriginalRelativePath = "Funds/1/Items/89/visible1-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('7', 64),
            PreviewStoredFileName = "visible1-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/89/visible1-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        var visible2 = new DigitalFile
        {
            Id = 814,
            ItemId = item.Id,
            OriginalFileName = "visible2.tif",
            OriginalStoredFileName = "visible2-stored.tif",
            OriginalRelativePath = "Funds/1/Items/89/visible2-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('8', 64),
            PreviewStoredFileName = "visible2-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/89/visible2-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = true,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.AddRange(hidden, visible1, visible2);
        await _context.SaveChangesAsync();

        var result = await _service.GetPreviewPageAsync(
            visible1.Id,
            enforceUserAccess: true,
            canDownloadOriginal: false,
            canDownloadPreview: true,
            backToItemDetailsArea: "User");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Position, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.PreviousFileId, Is.Null);
        Assert.That(result.NextFileId, Is.EqualTo(visible2.Id));
        Assert.That(result.PreviewImageUrl, Is.EqualTo($"/User/DigitalFiles/PreviewImage/{visible1.Id}"));
    }

    [Test]
    public async Task SetDownloadAllowedAsync_ShouldReturnFailure_WhenFileDoesNotExist()
    {
        var result = await _service.SetDownloadAllowedAsync(999, true);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("File not found."));
    }

    [Test]
    public async Task SetDownloadAllowedAsync_ShouldUpdateFlag_WhenFileExists()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 90);

        var digitalFile = new DigitalFile
        {
            Id = 815,
            ItemId = item.Id,
            OriginalFileName = "set-allowed.tif",
            OriginalStoredFileName = "set-allowed-stored.tif",
            OriginalRelativePath = "Funds/1/Items/90/set-allowed-stored.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 10,
            OriginalChecksumSha256 = new string('9', 64),
            PreviewStoredFileName = "set-allowed-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/90/set-allowed-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 5,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.SetDownloadAllowedAsync(digitalFile.Id, true);
        var updated = await _context.DigitalFiles.SingleAsync(df => df.Id == digitalFile.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(updated.IsDownloadAllowed, Is.True);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnFailure_WhenFileDoesNotExist()
    {
        var result = await _service.DeleteAsync(999);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("File not found."));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveDatabaseRecord_AndStoredFiles_WhenTheyExist()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 91);

        var originalPath = "Funds/1/Items/91/delete-original.tif";
        var previewPath = "Funds/1/Items/91/delete-preview.jpg";

        _originalStorage.Files[originalPath] = new byte[] { 1, 2, 3 };
        _previewStorage.Files[previewPath] = new byte[] { 4, 5, 6 };

        var digitalFile = new DigitalFile
        {
            Id = 816,
            ItemId = item.Id,
            OriginalFileName = "delete-me.tif",
            OriginalStoredFileName = "delete-original.tif",
            OriginalRelativePath = originalPath,
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 3,
            OriginalChecksumSha256 = new string('a', 64),
            PreviewStoredFileName = "delete-preview.jpg",
            PreviewRelativePath = previewPath,
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 3,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(digitalFile.Id);

        var existsInDb = await _context.DigitalFiles.AnyAsync(df => df.Id == digitalFile.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(existsInDb, Is.False);

        Assert.That(_originalStorage.DeleteCalled, Is.True);
        Assert.That(_previewStorage.DeleteCalled, Is.True);
        Assert.That(_originalStorage.Exists(originalPath), Is.False);
        Assert.That(_previewStorage.Exists(previewPath), Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveDatabaseRecord_EvenWhenStoredFilesAreAlreadyMissing()
    {
        var item = await TestDataSeeder.AddItemAsync(_context, id: 92);

        var digitalFile = new DigitalFile
        {
            Id = 817,
            ItemId = item.Id,
            OriginalFileName = "missing-stored-files.tif",
            OriginalStoredFileName = "missing-original.tif",
            OriginalRelativePath = "Funds/1/Items/92/missing-original.tif",
            OriginalContentType = "image/tiff",
            OriginalSizeBytes = 3,
            OriginalChecksumSha256 = new string('b', 64),
            PreviewStoredFileName = "missing-preview.jpg",
            PreviewRelativePath = "Funds/1/Items/92/missing-preview.jpg",
            PreviewContentType = "image/jpeg",
            PreviewSizeBytes = 3,
            IsDownloadAllowed = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.DigitalFiles.Add(digitalFile);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(digitalFile.Id);

        var existsInDb = await _context.DigitalFiles.AnyAsync(df => df.Id == digitalFile.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(existsInDb, Is.False);
    }
}