using FlowerShop.Dtos;

namespace FlowerShop.Services
{
    public interface IFlowerService
    {
        Task<List<FlowerDto>> GetAllAsync(int? categoryId, string? search, string sort);
        Task<FlowerDto?> GetByIdAsync(int id);
        Task<FlowerDto> CreateAsync(FlowerRequest request, int ownerId);
        Task<OperationResult> UpdateAsync(int id, FlowerRequest request, int userId, bool isAdmin);
        Task<OperationResult> DeleteAsync(int id, int userId, bool isAdmin);
    }
}
