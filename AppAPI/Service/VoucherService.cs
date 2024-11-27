using AppAPI.IRepository;
using AppAPI.IService;
using AppData.Model;

namespace AppAPI.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepo _repository;
        public VoucherService(IVoucherRepo repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<List<Voucher>> GetVouchersAsync()
        {
            try
            {
                return await _repository.GetVouchersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ServiceVoucher.GetVouchersAsync: {ex.Message}");
                return new List<Voucher>();
            }
        }
        public async Task<Voucher> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                return await _repository.GetVoucherByIdAsync(id);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in ServiceVoucher.GetVoucherByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Voucher voucher)
        {
            try
            {
                if (voucher == null)
                    throw new ArgumentNullException(nameof(voucher));

                // Additional validation
                if (string.IsNullOrEmpty(voucher.MaVoucher))
                    throw new ArgumentException("MaVoucher is required");

                if (voucher.NgayBatDau >= voucher.NgayKetThuc)
                    throw new ArgumentException("NgayBatDau must be before NgayKetThuc");

                return await _repository.CreateAsync(voucher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ServiceVoucher.CreateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in ServiceVoucher.DeleteAsync: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> UpdateAsync(Voucher voucher)
        {
            try
            {
                return await _repository.UpdateAsync(voucher);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in ServiceVoucher.UpdateAsync: {ex.Message}");
                return false;
            }
        }
    }
}
