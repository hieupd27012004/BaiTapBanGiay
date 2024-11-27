﻿using AppData.Model;

namespace APPMVC.IService
{
    public interface ISanPhamService
    {
        Task<List<SanPham>> GetSanPhams(string? name);
        Task<SanPham?> GetSanPhamById(Guid id);
        Task Create(SanPham sanPham);
        Task Update(SanPham sanPham);
        Task Delete(Guid id);
        Task<ThuongHieu?> GetThuongHieuBySanPhamIdAsync(Guid sanPhamId);
        Task<List<SanPham>> GetSanPhamClient(string? name);
    }
}
