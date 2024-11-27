using AppData.Model;

namespace AppAPI.IRepository
{
	public interface IGioHangChiTietRepo
	{
		Task<List<GioHangChiTiet>> GetAllAsync();
		Task<GioHangChiTiet> GetByIdAsync(Guid id);
		Task AddAsync(GioHangChiTiet gioHangChiTiet);
		Task UpdateAsync(GioHangChiTiet gioHangChiTiet);
		Task DeleteAsync(Guid id);

        Task<List<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId);

        Task ClearCartByIdAsync(Guid cartId);

		Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid cartId);
    }
}
