namespace DigitalisationManager.Services.Core.Contracts
{
    using DigitalisationManager.Web.ViewModels.Item;


    public interface IItemService
    {
        Task<ItemQueryViewModel> GetIndexAsync(int? fundId, string? q, int page, int pageSize);
        Task<ItemDetailsViewModel?> GetDetailsAsync(int id);

        Task<ItemFormViewModel> GetCreateAsync(int? fundId);

        Task<int> CreateAsync(ItemFormViewModel model);

        Task<ItemFormViewModel?> GetForEditAsync(int id);

        Task<(bool Success, string? Error)> UpdateAsync(ItemFormViewModel model);

        Task<ItemDetailsViewModel?> GetForDeleteAsync(int id);

        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
