using AppData.Model;

namespace AppAPI.IService
{
    public interface IHoaDonService
    {
        Task<List<HoaDon>> GetAllAsync(); 
        Task<HoaDon> GetByIdAsync(Guid id);
        Task AddAsync(HoaDon hoaDon);
        Task<bool> UpdateAsync(HoaDon hoaDon);
        Task DeleteAsync(Guid id);

        Task<HoaDon> GetByOrderNumberAsync(string orderNumber);
    }
}
