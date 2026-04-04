namespace DigitalisationManager.Web.ViewModels.Item
{
    public class ItemHistoryListViewModel
    {
        public int Id { get; set; }

        public string Action { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}