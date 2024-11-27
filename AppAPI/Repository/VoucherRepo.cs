using AppAPI.IRepository;
using AppData;
using AppData.Model;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Repository
{
    public class VoucherRepo : IVoucherRepo
    {
        private readonly AppDbcontext _context;
        public VoucherRepo(AppDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<Voucher>> GetVouchersAsync()
        {
            try
            {
                return await _context.vouchers.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VoucherRepo.GetVouchersAsync: {ex.Message}");
                return new List<Voucher>();
            }
        }

        public async Task<Voucher> GetVoucherByIdAsync(Guid id)
        {
            return await _context.vouchers.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Voucher voucher)
        {
            try
            {
                if (voucher == null)
                    throw new ArgumentNullException(nameof(voucher));

                _context.vouchers.Add(voucher);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VoucherRepo.CreateAsync: {ex.Message}");
                throw; // Rethrow to handle in upper layers
            }
        }
        public async Task<bool> UpdateAsync(Voucher voucher)
        {
            try
            {
                var existingVoucher = await _context.vouchers.FindAsync(voucher.VoucherId);
                if (existingVoucher == null)
                {
                    return false;
                }

                // Update properties
                existingVoucher.MaVoucher = voucher.MaVoucher;
                existingVoucher.MoTaVoucher = voucher.MoTaVoucher;
                existingVoucher.LoaiGiamGia = voucher.LoaiGiamGia;
                existingVoucher.GiaTriGiam = voucher.GiaTriGiam;
                existingVoucher.GiaTriDonHangToiThieu = voucher.GiaTriDonHangToiThieu;
                existingVoucher.SoTienToiDa = voucher.SoTienToiDa;
                existingVoucher.NgayBatDau = voucher.NgayBatDau;
                existingVoucher.NgayKetThuc = voucher.NgayKetThuc;
                existingVoucher.TongSoLuongVoucher = voucher.TongSoLuongVoucher;
                existingVoucher.SoLuongVoucherConLai = voucher.SoLuongVoucherConLai;
                existingVoucher.TrangThai = voucher.TrangThai;
                existingVoucher.NgayUpdate = voucher.NgayUpdate;
                existingVoucher.NguoiUpdate = voucher.NguoiUpdate;

                _context.vouchers.Update(existingVoucher);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VoucherRepo.UpdateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var voucher = await _context.vouchers.FindAsync(id);
                if (voucher != null)
                {
                    _context.vouchers.Remove(voucher);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;   
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in VoucherRepo.DeleteAsync: {ex.Message}");
                return false;
            }
        }
    }
}
