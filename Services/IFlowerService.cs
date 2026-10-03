using FlowerShop.Dtos;

namespace FlowerShop.Services
{
    public interface IFlowerService
    {
        Task<List<FlowerDto>> GetAllAsync(int? categoryId, string? search, string sort);
        Task<FlowerDto?> GetByIdAsync(int id);
        Task<FlowerDto> CreateAsync(FlowerRequest request);
        Task<bool> UpdateAsync(int id, FlowerRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
