using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Repository
{
	public class GioHangChiTietRepo : IGioHangChiTietRepo
	{
		private readonly AppDbcontext _context;

		public GioHangChiTietRepo(AppDbcontext context)
		{
			_context = context;
		}

		public async Task<List<GioHangChiTiet>> GetAllAsync()
		{
			return await _context.gioHangChiTiets.ToListAsync();
		}

		public async Task<GioHangChiTiet> GetByIdAsync(Guid id)
		{
			return await _context.gioHangChiTiets.FindAsync(id);
		}

		public async Task AddAsync(GioHangChiTiet gioHangChiTiet)
		{
			await _context.gioHangChiTiets.AddAsync(gioHangChiTiet);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(GioHangChiTiet gioHangChiTiet)
		{
			_context.gioHangChiTiets.Update(gioHangChiTiet);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var gioHangChiTiet = await GetByIdAsync(id);
			if (gioHangChiTiet != null)
			{
				_context.gioHangChiTiets.Remove(gioHangChiTiet);
				await _context.SaveChangesAsync();
			}
		}

        public async Task<List<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId)
        {
            return await _context.gioHangChiTiets
            .Where(x => x.IdGioHang == gioHangId) 
            .ToListAsync();
        }

        public async Task ClearCartByIdAsync(Guid cartId) 
        {
            var cartItems = await _context.gioHangChiTiets
            .Where(item => item.IdGioHang == cartId) 
            .ToListAsync(); 

            _context.gioHangChiTiets.RemoveRange(cartItems); 

            await _context.SaveChangesAsync(); 
        }

        public async Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid cartId)
        {
            var cartDetails = await _context.gioHangChiTiets
			   .Where(c => c.IdGioHang == cartId && c.IdSanPhamChiTiet == sanPhamChiTietId)
			   .ToListAsync();

            return cartDetails.Sum(c => c.SoLuong);
        }
    }
}
