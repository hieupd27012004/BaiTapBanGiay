using AppData.Model;

namespace AppAPI.IRepository
{
    public interface IVoucherRepo
    {
        Task<List<Voucher>> GetVouchersAsync();
        Task<Voucher> GetVoucherByIdAsync(Guid id);
        Task<bool> CreateAsync(Voucher voucher);
        Task<bool> UpdateAsync(Voucher voucher);
        Task<bool> DeleteAsync(Guid id);
    }
}
