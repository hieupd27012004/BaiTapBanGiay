using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Repository
{
    public class KhachHangRepo : IKhachHangRepo
    {
        private readonly AppDbcontext _context;
        public KhachHangRepo()
        {
            _context = new AppDbcontext();
        }
        public async Task<KhachHang> CreateKH(KhachHang kh)
        {
            kh.IdKhachHang = Guid.NewGuid();
            GioHang gioHang = new GioHang() 
            {
                IdGioHang = Guid.NewGuid(),
                IdKhachHang = kh.IdKhachHang,
            };
            kh.GioHang = gioHang;

                _context.khachHangs.Add(kh);
                await _context.SaveChangesAsync();
            return kh;
        }

        public async Task DeleteKH(Guid id)
        {
            var khachHang = await _context.khachHangs.FindAsync(id);
            if(khachHang != null )
            {
                _context.khachHangs.Remove(khachHang);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<KhachHang>> GetAllKhachHang()
        {
            return await _context.khachHangs.ToListAsync();
        }

        public async Task<KhachHang> GetByEmail(string email)
        {
            return await _context.khachHangs.FirstOrDefaultAsync(kh => kh.Email == email);
        }

        public async Task<KhachHang> GetBySDT(string sdT)
        {
            return await _context.khachHangs.FirstOrDefaultAsync(kh => kh.SoDienThoai == sdT);
        }

        public async Task<KhachHang> GetIdKhachHang(Guid id)
        {
            return await _context.khachHangs.FindAsync(id);
        }

        public async Task<KhachHang> UpdateKH(KhachHang kh)
        {

            _context.Entry(kh).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return kh;
        }
        public async Task<KhachHang> UpdateKHThongTin(KhachHang kh)
        {

            var khachHang = _context.khachHangs.Find(kh.IdKhachHang);
            if(khachHang !=  null)
            {
                khachHang.HoTen = kh.HoTen;
                khachHang.SoDienThoai = kh.SoDienThoai;
                khachHang.Email = kh.Email;
                khachHang.NgayCapNhat = DateTime.Now;
                _context.Update(kh);
                _context.SaveChangesAsync();
                return kh;

            }
            return kh;
        }
        public async Task<bool> ChangePassword(Guid id,string newPassword)
        {

            var ChangePassword = _context.khachHangs.Find(id);
            if(ChangePassword == null) return false;
            ChangePassword.MatKhau = newPassword;
            _context.khachHangs.Update(ChangePassword);
            _context.SaveChangesAsync();
            return true;    
        }
        public async Task<bool> ResetPass(string email, string newPassword)
        {
            var khachHang = _context.khachHangs.FirstOrDefault(e => e.Email == email);
            if (khachHang == null) return false;
            khachHang.MatKhau = newPassword;
            _context.khachHangs.Update(khachHang);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<KhachHang?> Login(string email, string password)
        {
            var kh = await _context.khachHangs.FirstOrDefaultAsync(kh => kh.Email == email);
            if (kh == null)
            {
                throw new Exception("Email Không tồn tại");
            }
            //Kiểm tra mk
            if(kh.MatKhau != password)
            {
                throw new Exception("Mật Khẩu Không Chính Xác");
            }
            
            //Hợp lệ trả về kh
            return kh;  
        }
        
    }
}
