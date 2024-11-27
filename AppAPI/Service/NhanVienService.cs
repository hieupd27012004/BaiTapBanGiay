using AppAPI.IRepository;
using AppAPI.IService;
using AppAPI.Repository;
using AppData.Model;

namespace AppAPI.Service
{
    public class NhanVienService : INhanVienService
    {
        private readonly INhanVienRepo _repo;
        public NhanVienService(INhanVienRepo repo)
        {
            _repo = repo;
        }
        public async Task<NhanVien?> Login(string email, string password)
        {
            return await _repo.Login(email, password);
        }
        public async Task<NhanVien> CreateNV(NhanVien nv)
        {
             await _repo.CreateNV(nv);
            return nv;
        }

        public async Task DeleteDM(Guid id)
        {
            await _repo.DeleteDM(id);
        }

        public async Task<List<NhanVien>> GetAllNhaVien()
        {
            return await _repo.GetAllNhaVien();
        }

        public async Task<NhanVien> GetIdNhanVien(Guid id)
        {
           return await _repo.GetIdNhanVien(id);
        }

        public async Task<NhanVien> UpdateNV(NhanVien nv)
        {
           return await _repo.UpdateNV(nv);
        }
        public async Task<NhanVien> UpdateThongTin(NhanVien nv)
        {
            return await _repo.UpdateThongTin(nv);
        }
        public async Task<bool> DoiMK(Guid idNhanVien, string newPassword)
        {
            return await _repo.DoiMK(idNhanVien, newPassword);
        }
        public async Task<bool> ResetPass(string email, string newPassword)
        {
            return await _repo.ResetPass(email, newPassword);
        }
    }
}
