
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
}