
namespace DigitalisationManager.Tests.Services;

using DigitalisationManager.Tests.Helpers;
using DigitalisationManager.Services.Core;
using DigitalisationManager.Web.ViewModels.Item;

[TestFixture]
public class ItemServiceTests
{
    private DigitalisationManagerDbContext _context = null!;
    private ItemService _service = null!;

    [SetUp]
    public async Task Setup()
    {
        _context = TestDbContextFactory.Create();
        await TestDataSeeder.SeedBasicAsync(_context);
        _service = new ItemService(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_ShouldCreateItem_WhenModelIsValid()
    {
        var model = new ItemFormViewModel
        {
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-1001",
            Description = "Test description",
            DocumentDateText = "1942",
            Status = ItemStatus.New
        };

        var createdId = await _service.CreateAsync(model);

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == createdId);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.FundId, Is.EqualTo(1));
        Assert.That(item.CategoryId, Is.EqualTo(1));
        Assert.That(item.InventoryNumber, Is.EqualTo("INV-1001"));
        Assert.That(item.Description, Is.EqualTo("Test description"));
        Assert.That(item.DocumentDateText, Is.EqualTo("1942"));
        Assert.That(item.Status, Is.EqualTo(ItemStatus.New));
    }

    [Test]
    public async Task CreateAsync_ShouldTrimValues_WhenServiceNormalizesInput()
    {
        var model = new ItemFormViewModel
        {
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "  INV-1002  ",
            Description = "  Trimmed description  ",
            DocumentDateText = "  1950  ",
            Status = ItemStatus.New
        };

        var createdId = await _service.CreateAsync(model);

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == createdId);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.InventoryNumber, Is.EqualTo("INV-1002"));
        Assert.That(item.Description, Is.EqualTo("Trimmed description"));
        Assert.That(item.DocumentDateText, Is.EqualTo("1950"));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenFundDoesNotExist()
    {
        var model = new ItemFormViewModel
        {
            FundId = 999,
            CategoryId = 1,
            InventoryNumber = "INV-1003",
            Description = "Missing fund",
            DocumentDateText = "1945",
            Status = ItemStatus.New
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(model));

        Assert.That(ex!.Message, Is.EqualTo("Selected fund does not exist."));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenCategoryIsInactive()
    {
        var model = new ItemFormViewModel
        {
            FundId = 1,
            CategoryId = 2,
            InventoryNumber = "INV-1004",
            Description = "Inactive category",
            DocumentDateText = "1946",
            Status = ItemStatus.New
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(model));

        Assert.That(ex!.Message, Is.EqualTo("Selected category does not exist or is inactive."));
    }

    [Test]
    public async Task CreateAsync_ShouldRejectDuplicateInventoryNumber_InSameFund()
    {
        await TestDataSeeder.AddItemAsync(
            _context,
            id: 10,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-2001");

        var model = new ItemFormViewModel
        {
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-2001",
            Description = "Duplicate number",
            DocumentDateText = "1955",
            Status = ItemStatus.New
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(model));

        Assert.That(ex!.Message, Is.EqualTo("Inventory number must be unique within the selected fund."));
    }

    [Test]
    public async Task CreateAsync_ShouldAllowSameInventoryNumber_InDifferentFund()
    {
        await TestDataSeeder.AddItemAsync(
            _context,
            id: 11,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-3001");

        var model = new ItemFormViewModel
        {
            FundId = 2,
            CategoryId = 1,
            InventoryNumber = "INV-3001",
            Description = "Same number, different fund",
            DocumentDateText = "1960",
            Status = ItemStatus.New
        };

        var createdId = await _service.CreateAsync(model);

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == createdId);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.FundId, Is.EqualTo(2));
        Assert.That(item.InventoryNumber, Is.EqualTo("INV-3001"));
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnFailure_WhenItemIdIsNull()
    {
        var model = new ItemFormViewModel
        {
            Id = null,
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-4001",
            Description = "Test description",
            DocumentDateText = "1940",
            Status = ItemStatus.New
        };

        var result = await _service.UpdateAsync(model);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("Item not found."));
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnFailure_WhenItemDoesNotExist()
    {
        var model = new ItemFormViewModel
        {
            Id = 999,
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-4002",
            Description = "Missing item",
            DocumentDateText = "1941",
            Status = ItemStatus.New
        };

        var result = await _service.UpdateAsync(model);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("Item not found."));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateItem_WhenModelIsValid()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 20,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-5001",
            description: "Old description",
            documentDateText: "1900",
            status: ItemStatus.New);

        var model = new ItemFormViewModel
        {
            Id = item.Id,
            FundId = 2,
            CategoryId = 3,
            InventoryNumber = "INV-5001-UPDATED",
            Description = "Updated description",
            DocumentDateText = "1901",
            Status = ItemStatus.InProgress
        };

        var result = await _service.UpdateAsync(model);

        var updatedItem = await _context.Items.FirstAsync(i => i.Id == item.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);

        Assert.That(updatedItem.FundId, Is.EqualTo(2));
        Assert.That(updatedItem.CategoryId, Is.EqualTo(3));
        Assert.That(updatedItem.InventoryNumber, Is.EqualTo("INV-5001-UPDATED"));
        Assert.That(updatedItem.Description, Is.EqualTo("Updated description"));
        Assert.That(updatedItem.DocumentDateText, Is.EqualTo("1901"));
        Assert.That(updatedItem.Status, Is.EqualTo(ItemStatus.InProgress));
    }

    [Test]
    public async Task UpdateAsync_ShouldCreateHistoryEntry_WhenDataIsChanged()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 21,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-5002",
            description: "Old description",
            documentDateText: "1910",
            status: ItemStatus.New);

        var model = new ItemFormViewModel
        {
            Id = item.Id,
            FundId = 2,
            CategoryId = 3,
            InventoryNumber = "INV-5002-UPDATED",
            Description = "New description",
            DocumentDateText = "1911",
            Status = ItemStatus.InProgress
        };

        var result = await _service.UpdateAsync(model);

        var historyEntries = await _context.ItemHistories
            .Where(h => h.ItemId == item.Id)
            .ToListAsync();

        Assert.That(result.Success, Is.True);
        Assert.That(historyEntries.Count, Is.EqualTo(1));

        var history = historyEntries[0];

        Assert.That(history.Action, Is.EqualTo("Item updated"));
        Assert.That(history.Description, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task UpdateAsync_ShouldDescribeAllChangedFields_InHistoryEntry()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 22,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-5003",
            description: "Old description",
            documentDateText: "1920",
            status: ItemStatus.New);

        var model = new ItemFormViewModel
        {
            Id = item.Id,
            FundId = 2,
            CategoryId = 3,
            InventoryNumber = "INV-5003-UPDATED",
            Description = "Updated description",
            DocumentDateText = "1921",
            Status = ItemStatus.InProgress
        };

        await _service.UpdateAsync(model);

        var history = await _context.ItemHistories
            .FirstAsync(h => h.ItemId == item.Id);

        Assert.That(history.Description, Does.Contain("Fund"));
        Assert.That(history.Description, Does.Contain("Category"));
        Assert.That(history.Description, Does.Contain("Inventory"));
        Assert.That(history.Description, Does.Contain("Description"));
        Assert.That(history.Description, Does.Contain("Document date").Or.Contain("Date"));
        Assert.That(history.Description, Does.Contain("Status"));
    }

    [Test]
    public async Task UpdateAsync_ShouldNotCreateHistoryEntry_WhenNothingIsChanged()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 23,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-5004",
            description: "Same description",
            documentDateText: "1930",
            status: ItemStatus.New);

        var model = new ItemFormViewModel
        {
            Id = item.Id,
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-5004",
            Description = "Same description",
            DocumentDateText = "1930",
            Status = ItemStatus.New
        };

        var result = await _service.UpdateAsync(model);

        var historyEntries = await _context.ItemHistories
            .Where(h => h.ItemId == item.Id)
            .ToListAsync();

        Assert.That(result.Success, Is.True);
        Assert.That(historyEntries, Is.Empty);
    }

    [Test]
    public async Task UpdateAsync_ShouldTrimUpdatedValues_WhenServiceNormalizesInput()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 24,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-5005",
            description: "Old",
            documentDateText: "1940",
            status: ItemStatus.New);

        var model = new ItemFormViewModel
        {
            Id = item.Id,
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "  INV-5005-UPDATED  ",
            Description = "  Updated trimmed description  ",
            DocumentDateText = "  1941  ",
            Status = ItemStatus.InProgress
        };

        var result = await _service.UpdateAsync(model);

        var updatedItem = await _context.Items.FirstAsync(i => i.Id == item.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(updatedItem.InventoryNumber, Is.EqualTo("INV-5005-UPDATED"));
        Assert.That(updatedItem.Description, Is.EqualTo("Updated trimmed description"));
        Assert.That(updatedItem.DocumentDateText, Is.EqualTo("1941"));
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnFailure_WhenItemDoesNotExist()
    {
        var result = await _service.DeleteAsync(999);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("Item not found."));
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnFailure_WhenItemHasDigitalFiles()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 30,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-6001");

        await TestDataSeeder.AddDigitalFileAsync(
            _context,
            id: 1,
            itemId: item.Id,
            originalFileName: "scan-001.tif");

        var result = await _service.DeleteAsync(item.Id);

        var itemStillExists = await _context.Items.AnyAsync(i => i.Id == item.Id);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("Cannot delete this item because it has uploaded TIFF files. Delete the files first."));
        Assert.That(itemStillExists, Is.True);
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteItem_WhenItemHasNoDigitalFiles()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 31,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-6002");

        var result = await _service.DeleteAsync(item.Id);

        var itemStillExists = await _context.Items.AnyAsync(i => i.Id == item.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(itemStillExists, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteOnlyRequestedItem_WhenOtherItemsExist()
    {
        var itemToDelete = await TestDataSeeder.AddItemAsync(
            _context,
            id: 32,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-6003");

        var otherItem = await TestDataSeeder.AddItemAsync(
            _context,
            id: 33,
            fundId: 2,
            categoryId: 3,
            inventoryNumber: "INV-6004");

        var result = await _service.DeleteAsync(itemToDelete.Id);

        var deletedExists = await _context.Items.AnyAsync(i => i.Id == itemToDelete.Id);
        var otherExists = await _context.Items.AnyAsync(i => i.Id == otherItem.Id);

        Assert.That(result.Success, Is.True);
        Assert.That(deletedExists, Is.False);
        Assert.That(otherExists, Is.True);
    }

    [Test]
    public async Task DeleteAsync_ShouldNotDeleteItemHistoryOrFilesImplicitly_WhenDeleteIsBlocked()
    {
        var item = await TestDataSeeder.AddItemAsync(
            _context,
            id: 34,
            fundId: 1,
            categoryId: 1,
            inventoryNumber: "INV-6005");

        _context.ItemHistories.Add(new ItemHistory
        {
            ItemId = item.Id,
            Action = "Item created",
            Description = "Initial history",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        await TestDataSeeder.AddDigitalFileAsync(
            _context,
            id: 2,
            itemId: item.Id,
            originalFileName: "scan-002.tif");

        var result = await _service.DeleteAsync(item.Id);

        var itemStillExists = await _context.Items.AnyAsync(i => i.Id == item.Id);
        var historyCount = await _context.ItemHistories.CountAsync(h => h.ItemId == item.Id);
        var fileCount = await _context.DigitalFiles.CountAsync(f => f.ItemId == item.Id);

        Assert.That(result.Success, Is.False);
        Assert.That(itemStillExists, Is.True);
        Assert.That(historyCount, Is.EqualTo(1));
        Assert.That(fileCount, Is.EqualTo(1));
    }
    [Test]
    public async Task GetIndexAsync_ShouldReturnAllItems_WhenNoFiltersAreApplied()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 100,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-A1",
                Description = "Letter from Sofia",
                DocumentDateText = "1920",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-3)
            },
            new Item
            {
                Id = 101,
                FundId = 1,
                CategoryId = 3,
                InventoryNumber = "INV-A2",
                Description = "Photo album",
                DocumentDateText = "1921",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 102,
                FundId = 2,
                CategoryId = 3,
                InventoryNumber = "INV-B1",
                Description = "Photo from Plovdiv",
                DocumentDateText = "1922",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: null, page: 1, pageSize: 10);

        Assert.That(result.FundId, Is.Null);
        Assert.That(result.Q, Is.Null);
        Assert.That(result.Results.TotalCount, Is.EqualTo(3));
        Assert.That(result.Results.Items.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetIndexAsync_ShouldFilterByFund()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 110,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-F1",
                Description = "Fund one item",
                DocumentDateText = "1930",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 111,
                FundId = 2,
                CategoryId = 1,
                InventoryNumber = "INV-F2",
                Description = "Fund two item",
                DocumentDateText = "1931",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: 1, q: null, page: 1, pageSize: 10);

        Assert.That(result.FundId, Is.EqualTo(1));
        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].FundId, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("INV-F1"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldTrimSearchText()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 120,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "PHOTO-001",
                Description = "Large TIFF image",
                DocumentDateText = "1940",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 121,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "LEDGER-001",
                Description = "Administrative ledger",
                DocumentDateText = "1941",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: "  PHOTO-001  ", page: 1, pageSize: 10);

        Assert.That(result.Q, Is.EqualTo("PHOTO-001"));
        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("PHOTO-001"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldFilterByInventoryNumber()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 130,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "PHOTO-INV-1",
                Description = "First description",
                DocumentDateText = "1950",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 131,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "LEDGER-INV-1",
                Description = "Second description",
                DocumentDateText = "1951",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: "PHOTO-INV", page: 1, pageSize: 10);

        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("PHOTO-INV-1"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldFilterByDescription()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 140,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-D1",
                Description = "UniqueDescriptionMarker",
                DocumentDateText = "1960",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 141,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-D2",
                Description = "Other text",
                DocumentDateText = "1961",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: "UniqueDescriptionMarker", page: 1, pageSize: 10);

        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("INV-D1"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldFilterByDocumentDateText()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 150,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-T1",
                Description = "Date text item",
                DocumentDateText = "SpecialDateMarker-1879",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 151,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-T2",
                Description = "Other date text item",
                DocumentDateText = "1880",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: "SpecialDateMarker", page: 1, pageSize: 10);

        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("INV-T1"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldFilterByCategoryName()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 160,
                FundId = 1,
                CategoryId = 3,
                InventoryNumber = "INV-CAT-1",
                Description = "Category search item",
                DocumentDateText = "1970",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 161,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-CAT-2",
                Description = "Other category item",
                DocumentDateText = "1971",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: "Correspondence", page: 1, pageSize: 10);

        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].CategoryName, Is.EqualTo("Correspondence"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldApplyFundAndSearchTogether()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 170,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "PHOTO-F1",
                Description = "Photo in fund one",
                DocumentDateText = "1980",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-3)
            },
            new Item
            {
                Id = 171,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "LEDGER-F1",
                Description = "Ledger in fund one",
                DocumentDateText = "1981",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 172,
                FundId = 2,
                CategoryId = 1,
                InventoryNumber = "PHOTO-F2",
                Description = "Photo in fund two",
                DocumentDateText = "1982",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: 1, q: "PHOTO", page: 1, pageSize: 10);

        Assert.That(result.FundId, Is.EqualTo(1));
        Assert.That(result.Q, Is.EqualTo("PHOTO"));
        Assert.That(result.Results.TotalCount, Is.EqualTo(1));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("PHOTO-F1"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldNormalizeInvalidPagingValues()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 180,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-P1",
                Description = "Paging test one",
                DocumentDateText = "1990",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new Item
            {
                Id = 181,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-P2",
                Description = "Paging test two",
                DocumentDateText = "1991",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: null, page: 0, pageSize: 0);

        Assert.That(result.Results.Page, Is.EqualTo(1));
        Assert.That(result.Results.PageSize, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetIndexAsync_ShouldClampPageToTotalPages_WhenRequestedPageIsTooLarge()
    {
        for (int i = 1; i <= 3; i++)
        {
            _context.Items.Add(new Item
            {
                Id = 190 + i,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = $"INV-CLAMP-{i}",
                Description = $"Clamp item {i}",
                DocumentDateText = $"200{i}",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: null, page: 99, pageSize: 2);

        Assert.That(result.Results.TotalCount, Is.EqualTo(3));
        Assert.That(result.Results.TotalPages, Is.EqualTo(2));
        Assert.That(result.Results.Page, Is.EqualTo(2));
        Assert.That(result.Results.Items.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetIndexAsync_ShouldReturnNewestItemsFirst()
    {
        _context.Items.AddRange(
            new Item
            {
                Id = 210,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-OLD",
                Description = "Old item",
                DocumentDateText = "2010",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
            new Item
            {
                Id = 211,
                FundId = 1,
                CategoryId = 1,
                InventoryNumber = "INV-NEW",
                Description = "New item",
                DocumentDateText = "2011",
                Status = ItemStatus.New,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: null, q: null, page: 1, pageSize: 10);

        Assert.That(result.Results.Items.Count, Is.EqualTo(2));
        Assert.That(result.Results.Items[0].InventoryNumber, Is.EqualTo("INV-NEW"));
        Assert.That(result.Results.Items[1].InventoryNumber, Is.EqualTo("INV-OLD"));
    }

    [Test]
    public async Task GetIndexAsync_ShouldReturnEmptyCollection_WhenNoItemsMatch()
    {
        _context.Items.Add(new Item
        {
            Id = 220,
            FundId = 1,
            CategoryId = 1,
            InventoryNumber = "INV-NOMATCH",
            Description = "Administrative record",
            DocumentDateText = "2020",
            Status = ItemStatus.New,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var result = await _service.GetIndexAsync(fundId: 2, q: "photo", page: 1, pageSize: 10);

        Assert.That(result.Results.TotalCount, Is.EqualTo(0));
        Assert.That(result.Results.TotalPages, Is.EqualTo(0));
        Assert.That(result.Results.Items, Is.Empty);
    }
}