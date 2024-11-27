using AppData.Model;

namespace AppAPI.IRepository
{
    public interface IKhachHangRepo
    {
        Task<List<KhachHang>> GetAllKhachHang();
        Task<KhachHang> GetIdKhachHang(Guid id);

        Task<KhachHang> CreateKH(KhachHang kh);
        Task<KhachHang> UpdateKH(KhachHang kh);
        Task<KhachHang> UpdateKHThongTin(KhachHang kh);
        Task<bool> ChangePassword(Guid id, string newPassword);
        Task<bool> ResetPass(string email, string newPassword);
        Task DeleteKH(Guid id);
        Task<KhachHang> GetByEmail(string email);
        Task<KhachHang> GetBySDT(string sdT);
        Task<KhachHang?> Login(string email, string password);
      
    }
}
