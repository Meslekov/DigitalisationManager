namespace DigitalisationManager.GCommon.Enums
{
    public enum ItemStatus
    {
        New = 0,
        InProgress = 1,   // In process of scanning (it can be a longer process like scanning a book)
        Digitized = 2,    // In process of converting (if it's needed)
        Archived = 3      // Processed and stored
    }
}
