using AppData.Model;
using APPMVC.IService;
using System.Net;

namespace APPMVC.Service
{
    public class HoaDonChiTietService : IHoaDonChiTietService
    {
        private readonly HttpClient _httpClient;

        public HoaDonChiTietService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7198") 
            };
        }

        public async Task AddAsync(List<HoaDonChiTiet> hoaDonChiTietList)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HoaDonChiTiet/them", hoaDonChiTietList); 
            response.EnsureSuccessStatusCode();
        }

        // Xóa chi tiết hóa đơn theo ID
        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/HoaDonChiTiet/xoa?id={id}");
            response.EnsureSuccessStatusCode();
        }

        // Lấy tất cả chi tiết hóa đơn
        public async Task<List<HoaDonChiTiet>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/HoaDonChiTiet/getall");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<HoaDonChiTiet>>();
        }

        // Lấy chi tiết hóa đơn theo ID
        public async Task<HoaDonChiTiet> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/HoaDonChiTiet/getbyid?id={id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HoaDonChiTiet>();
        }

        public async Task<List<HoaDonChiTiet>> GetByIdHoaDonAsync(Guid idHoaDon)
        {
            var response = await _httpClient.GetAsync($"api/HoaDonChiTiet/getbyidhd?idhoadon={idHoaDon}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Xử lý trường hợp không tìm thấy
                    return new List<HoaDonChiTiet>(); // Trả về danh sách rỗng
                }
                response.EnsureSuccessStatusCode(); // Ném ngoại lệ cho các lỗi khác
            }
            return await response.Content.ReadFromJsonAsync<List<HoaDonChiTiet>>();
        }

        // Cập nhật chi tiết hóa đơn
        public async Task UpdateAsync(List<HoaDonChiTiet> hoaDonChiTietList)
        {
            var response = await _httpClient.PutAsJsonAsync("api/HoaDonChiTiet/sua", hoaDonChiTietList);
            response.EnsureSuccessStatusCode();
        }

        public async Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid hDCTId)
        {
            var response = await _httpClient.GetAsync($"api/HoaDonChiTiet/gettotalquantity?sanPhamChiTietId={sanPhamChiTietId}&cartId={hDCTId}");

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();

            // Read the total quantity from the response
            var totalQuantity = await response.Content.ReadFromJsonAsync<double>();
            return totalQuantity;
        }
    }
}
