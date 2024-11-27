﻿using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore; // Đảm bảo bạn đã thêm namespace này
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Thêm namespace này để sử dụng Task

namespace AppAPI.Repository
{
    public class SanPhamChiTietKichCoRepo : ISanPhamChiTietKichCoRepo
    {
        private readonly AppDbcontext _context;

        public SanPhamChiTietKichCoRepo(AppDbcontext context)
        {
            _context = context;
        }

        // Sửa đổi phương thức này để sử dụng async/await
        public async Task<List<SanPhamChiTietKichCo>> GetSanPhamChiTietKichCoAsync()
        {
            return await _context.sanPhamChiTietKichCos
                .Include(s => s.SanPhamChiTiet)
                .Include(k => k.KichCo)
                .ToListAsync();
        }

        public async Task<SanPhamChiTietKichCo> GetKichCoByIdAsync(Guid id)
        {
            return await _context.sanPhamChiTietKichCos.FirstOrDefaultAsync(x => x.IdSanPhamChiTiet == id); 
        }

        public async Task<bool> CreateAsync(SanPhamChiTietKichCo kichCo)
        {
            await _context.sanPhamChiTietKichCos.AddAsync(kichCo); // Sử dụng AddAsync để thêm không đồng bộ
            return await _context.SaveChangesAsync() > 0; // Returns true if changes were saved
        }

        public async Task<bool> UpdateAsync(SanPhamChiTietKichCo kichCo)
        {
            _context.sanPhamChiTietKichCos.Update(kichCo);
            return await _context.SaveChangesAsync() > 0; // Returns true if changes were saved
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var kichCo = await _context.sanPhamChiTietKichCos.FindAsync(id); // Sử dụng FindAsync để tìm kiếm không đồng bộ
            if (kichCo != null)
            {
                _context.sanPhamChiTietKichCos.Remove(kichCo);
                return await _context.SaveChangesAsync() > 0; // Returns true if changes were saved
            }
            return false; // Return false if the item was not found
        }

        public async Task<List<KichCo>> GetKichCoIdsBySanPhamChiTietId(Guid sanPhamChiTietId)
        {

            return await _context.sanPhamChiTietKichCos
                .Where(x => x.IdSanPhamChiTiet == sanPhamChiTietId )
                .Select(x => x.KichCo)
                .ToListAsync();


        }
    }
}