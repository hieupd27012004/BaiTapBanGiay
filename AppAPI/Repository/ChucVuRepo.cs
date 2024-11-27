using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Repository
{
    public class ChucVuRepo : IChucVuRepo
    {
        AppDbcontext _context;
        public ChucVuRepo()
        {
            _context = new AppDbcontext();
        }

        public async Task<ChucVu> CreateCV(ChucVu cv)
        {
            _context.chuVu.Add(cv);
            await _context.SaveChangesAsync();
            return cv;

        }

        public async Task DeleteCV(Guid id)
        {
           var chucvu = await _context.chuVu.FindAsync(id);
            if(chucvu != null)
            {
                _context.chuVu.Remove(chucvu);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ChucVu>> GetAllChucVu()
        {
            return await _context.chuVu.ToListAsync();
        }

        public async Task<ChucVu> GetIdChucVu(Guid id)
        {
            return await _context.chuVu.FindAsync(id);
        }

        public async Task<ChucVu> UpdateCV(ChucVu cv)
        {
            _context.chuVu.Update(cv);
            await _context.SaveChangesAsync();
            return cv;
        }
    }
}
