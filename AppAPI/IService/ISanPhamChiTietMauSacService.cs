using AppData.Model;

namespace AppAPI.IService
{
    public interface ISanPhamChiTietMauSacService
    {
        List<SanPhamChiTietMauSac> GetSanPhamChiTietMauSac();
        SanPhamChiTietMauSac GetMauSacById(Guid id);

        // Create a new Mau Sac
        bool Create(SanPhamChiTietMauSac mauSac);

        // Update an existing Mau Sac
        bool Update(SanPhamChiTietMauSac mauSac);

        // Delete a Mau Sac by ID
        bool Delete(Guid id);

        Task<List<MauSac>> GetMauSacIdsBySanPhamChiTietId(Guid sanPhamChiTietId);

    }
}
