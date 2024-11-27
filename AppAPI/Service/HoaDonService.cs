using AppAPI.IRepository;
using AppAPI.IService;
using AppData.Model;

namespace AppAPI.Service
{
    public class HoaDonService : IHoaDonService
    {
        private readonly IHoaDonRepo _hoaDonRepository;

        public HoaDonService(IHoaDonRepo hoaDonRepository)
        {
            _hoaDonRepository = hoaDonRepository;
        }

        public async Task<List<HoaDon>> GetAllAsync()  
        {
            return await _hoaDonRepository.GetAllAsync();
        }

        public async Task<HoaDon> GetByIdAsync(Guid id)
        {
            return await _hoaDonRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(HoaDon hoaDon)
        {
            await _hoaDonRepository.AddAsync(hoaDon);
        }

        public async Task<bool> UpdateAsync(HoaDon hoaDon)
        {
            try
            {
                return await _hoaDonRepository.UpdateAsync(hoaDon);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the product: {ex.Message}");
                return false; 
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await _hoaDonRepository.DeleteAsync(id);
        }

        public async Task<HoaDon> GetByOrderNumberAsync(string orderNumber)
        {
            return await _hoaDonRepository.GetByOrderNumberAsync(orderNumber);
        }
    }
}