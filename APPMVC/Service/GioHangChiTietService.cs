using AppData.Model;
using APPMVC.IService;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace APPMVC.Service
{
	public class GioHangChiTietService : IGioHangChiTietService
	{
		private readonly HttpClient _httpClient;

		public GioHangChiTietService()
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://localhost:7198")
			};
		}

		// Thêm giỏ hàng chi tiết
		public async Task AddAsync(GioHangChiTiet gioHangChiTiet)
		{
			var response = await _httpClient.PostAsJsonAsync("api/GioHangChiTiet/them", gioHangChiTiet);
			response.EnsureSuccessStatusCode();
		}

        public async Task ClearCartByIdAsync(Guid cartId)
        {
            var response = await _httpClient.DeleteAsync($"api/GioHangChiTiet/clear?cartId={cartId}");
            response.EnsureSuccessStatusCode(); 
        }

        // Xóa giỏ hàng chi tiết theo ID
        public async Task DeleteAsync(Guid id)
		{
			var response = await _httpClient.DeleteAsync($"api/GioHangChiTiet/xoa?id={id}");
			response.EnsureSuccessStatusCode();
		}

		// Lấy tất cả giỏ hàng chi tiết
		public async Task<List<GioHangChiTiet>> GetAllAsync()
		{
			var response = await _httpClient.GetAsync("api/GioHangChiTiet/getall");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<GioHangChiTiet>>();
		}

        public async Task<List<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId)
        {
            // Kiểm tra xem gioHangId có hợp lệ không
            if (gioHangId == Guid.Empty)
            {
                throw new ArgumentException("Invalid cart ID.", nameof(gioHangId));
            }

            var response = await _httpClient.GetAsync($"api/GioHangChiTiet/getbygiohangid?gioHangId={gioHangId}");

            // Kiểm tra phản hồi
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<GioHangChiTiet>();
                }

                throw new Exception($"Error retrieving cart details: {response.ReasonPhrase}. Content: {content}");
            }

            try
            {
                // Đọc nội dung JSON
                var jsonContent = await response.Content.ReadAsStringAsync();

                // Deserialize JSON thành danh sách GioHangChiTiet
                var gioHangChiTiets = JsonConvert.DeserializeObject<List<GioHangChiTiet>>(jsonContent);

                return gioHangChiTiets ?? new List<GioHangChiTiet>();
            }
            catch (Newtonsoft.Json.JsonException jsonEx) // Chỉ định rõ namespace
            {
                throw new Exception("Error parsing JSON response.", jsonEx);
            }
        }

        // Lấy giỏ hàng chi tiết theo ID
        public async Task<GioHangChiTiet> GetByIdAsync(Guid id)
		{
			var response = await _httpClient.GetAsync($"api/GioHangChiTiet/getbyid?id={id}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<GioHangChiTiet>();
		}

        public async Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid cartId)
        {
            var response = await _httpClient.GetAsync($"api/GioHangChiTiet/gettotalquantity?sanPhamChiTietId={sanPhamChiTietId}&cartId={cartId}");

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();

            // Read the total quantity from the response
            var totalQuantity = await response.Content.ReadFromJsonAsync<double>();
            return totalQuantity;
        }

        // Cập nhật giỏ hàng chi tiết
        public async Task UpdateAsync(GioHangChiTiet gioHangChiTiet)
		{
			var response = await _httpClient.PutAsJsonAsync("api/GioHangChiTiet/sua", gioHangChiTiet);
			response.EnsureSuccessStatusCode();
		}
	}
}

public class GioHangChiTiet
{
    public Guid IdGioHangChiTiet { get; set; }
    public double DonGia { get; set; }
    public double SoLuong { get; set; }
    public double TongTien { get; set; }
    public int KichHoat { get; set; }
    public Guid IdGioHang { get; set; }
    public Guid IdSanPhamChiTiet { get; set; }
}