namespace DigitalisationManager.Tests.Helpers;

public static class TestDbContextFactory
{
    public static DigitalisationManagerDbContext Create()
    {
        var options = new DbContextOptionsBuilder<DigitalisationManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DigitalisationManagerDbContext(options);
    }
}