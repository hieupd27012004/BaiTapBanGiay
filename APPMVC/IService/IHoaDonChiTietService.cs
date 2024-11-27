﻿using AppData.Model;

namespace APPMVC.IService
{
    public interface IHoaDonChiTietService
    {
        Task<List<HoaDonChiTiet>> GetAllAsync();
        Task<HoaDonChiTiet> GetByIdAsync(Guid id);
        Task AddAsync(List<HoaDonChiTiet> hoaDonChiTietList);
        Task UpdateAsync(List<HoaDonChiTiet> hoaDonChiTietList);
        Task DeleteAsync(Guid id);

        Task<List<HoaDonChiTiet>> GetByIdHoaDonAsync(Guid idHoaDon);
        Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid hDCTId);
    }
}
