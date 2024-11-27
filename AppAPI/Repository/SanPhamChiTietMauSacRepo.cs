using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppAPI.Repository
{
    public class SanPhamChiTietMauSacRepo : ISanPhamChiTietMauSacRepo
    {
        private readonly AppDbcontext _context;

        public SanPhamChiTietMauSacRepo(AppDbcontext context)
        {
            _context = context;
        }

        public List<SanPhamChiTietMauSac> GetSanPhamChiTietMauSac()
        {
            return _context.sanPhamChiTietMausacs.ToList();
        }

        public SanPhamChiTietMauSac GetMauSacById(Guid id)
        {
            return _context.sanPhamChiTietMausacs.Find(id);
        }

        public bool Create(SanPhamChiTietMauSac mauSac)
        {
            _context.sanPhamChiTietMausacs.Add(mauSac);
            return _context.SaveChanges() > 0; // Returns true if changes were saved
        }

        public bool Update(SanPhamChiTietMauSac mauSac)
        {
            _context.sanPhamChiTietMausacs.Update(mauSac);
            return _context.SaveChanges() > 0; // Returns true if changes were saved
        }

        public bool Delete(Guid id)
        {
            var mauSac = _context.sanPhamChiTietMausacs.Find(id);
            if (mauSac != null)
            {
                _context.sanPhamChiTietMausacs.Remove(mauSac);
                return _context.SaveChanges() > 0; // Returns true if changes were saved
            }
            return false; // Return false if the item was not found
        }

        public async Task<List<MauSac>> GetMauSacIdsBySanPhamChiTietId(Guid sanPhamChiTietId)
        {
            return  await _context.sanPhamChiTietMausacs
             .Where(m => m.IdSanPhamChiTiet == sanPhamChiTietId)
             .Select(m => m.MauSac)
             .ToListAsync();          
        }
    }
}