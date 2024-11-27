using AppData.Model;
using APPMVC.IService;

namespace APPMVC.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly HttpClient _httpClient;
        public VoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://localhost:7198");
        }

        public async Task<bool> CreateAsync(Voucher voucher)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Voucher/CreateVoucher", voucher);

                // Log response
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create Response: Status: {response.StatusCode}, Content: {content}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Voucher/DeleteVoucher?id={id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex) 
            {
                Console.WriteLine($"HTTP request error when deleting voucher: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error when deleting voucher: {ex.Message}");
                return false;
            }
        }

        public async Task<Voucher> GetVoucherByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/Voucher/GetVoucherById?id={id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Voucher>(); 
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error when getting voucher by ID: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when getting voucher by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Voucher>> GetVouchersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Voucher>>("api/Voucher/GetAllVouchers") ?? new List<Voucher>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error when getting all vouchers: {ex.Message}");
                return new List<Voucher>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when getting all vouchers: {ex.Message}");
                return new List<Voucher>();
            }
        }

        public async Task<bool> UpdateAsync(Voucher voucher)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Voucher/UpdateVoucher/{voucher.VoucherId}", voucher);

                // Add debug logging
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update Response: Status: {response.StatusCode}, Content: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Update failed with status code: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VoucherService.UpdateAsync: {ex.Message}");
                return false;
            }
        }
    }
}
