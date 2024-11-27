using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Repository
{
    public class HoaDonChiTietRepo : IHoaDonChiTietRepo
    {
        private readonly AppDbcontext _context;

        public HoaDonChiTietRepo(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<List<HoaDonChiTiet>> GetAllAsync()
        {
            return await _context.hoaDonChiTiets.ToListAsync();
        }

        public async Task<HoaDonChiTiet> GetByIdAsync(Guid id)
        {
            return await _context.hoaDonChiTiets.FindAsync(id);
        }

        public async Task AddAsync(List<HoaDonChiTiet> hoaDonChiTietList)
        {
            await _context.hoaDonChiTiets.AddRangeAsync(hoaDonChiTietList); 
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(List<HoaDonChiTiet> hoaDonChiTietList)
        {
            _context.hoaDonChiTiets.UpdateRange(hoaDonChiTietList);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var hoaDonChiTiet = await GetByIdAsync(id);
            if (hoaDonChiTiet != null)
            {
                _context.hoaDonChiTiets.Remove(hoaDonChiTiet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<HoaDonChiTiet>> GetByIdHoaDonAsync(Guid idHoaDon)
        {
            return await _context.hoaDonChiTiets
                .Where(h => h.IdHoaDon == idHoaDon) 
                .ToListAsync();
        }

        public async Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid hDCTId)
        {
            var cartDetails = await _context.hoaDonChiTiets
               .Where(c => c.IdHoaDonChiTiet == hDCTId && c.IdSanPhamChiTiet == sanPhamChiTietId)
               .ToListAsync();

            return cartDetails.Sum(c => c.SoLuong);
        }
    }
}