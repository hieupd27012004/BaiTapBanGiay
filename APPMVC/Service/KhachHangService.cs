using AppData.Model;
using APPMVC.IService;
using Newtonsoft.Json;
using System.Text;

namespace APPMVC.Service
{
    public class KhachHangService : IKhachHangService
    {
        HttpClient _httpClient;
        public KhachHangService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7198");
        }
        public async Task AddKhachHang(KhachHang kh)
        {
             await _httpClient.PostAsJsonAsync("/api/KhachHang/DangKy", kh);          
        }
        public async Task<KhachHang?> LoginKH(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/KhachHang/Login", new { Email = email, MatKhau = password });
            if(response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Deserializing chuỗi JSON thành đối tượng KhachHang
                return JsonConvert.DeserializeObject<KhachHang>(responseContent);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }
        public async Task DeleteKhachHang(Guid id)
        {
            await _httpClient.DeleteAsync($"api/KhachHang/Xoa?id={id}");
        }

        public async Task<List<KhachHang>> GetAllKhachHang()
        {
            return await _httpClient.GetFromJsonAsync<List<KhachHang>>("/api/KhachHang/GetAllKhachHang");
        }

        public Task<KhachHang> GetIdKhachHang(Guid? id)
        {
            var getkh = _httpClient.GetFromJsonAsync<KhachHang>($"/api/KhachHang/GetById?id={id}");
            return getkh;
        }

        public async Task UpdateKhachHang(KhachHang kh)
        {
            await _httpClient.PutAsJsonAsync("api/KhachHang/Sua", kh);
        }
        public async Task UpdateKHThongTin(KhachHang kh)
        {
            await _httpClient.PutAsJsonAsync("api/KhachHang/SuaChoKhachHang", kh);
        }
        public async Task<bool> ChangePassword(Guid? id, string newPassword, string confirmPassword)
        {
            var response = await _httpClient.PostAsync($"api/KhachHang/DoiMK?id={id}&newPassword={newPassword}&confirmPassword={confirmPassword}", new StringContent(JsonConvert.SerializeObject(new { newPassword }), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return false;
            }
            var response = await _httpClient.PostAsync($"api/KhachHang/ResetPassword?email={email}&newPassword={newPassword}&confirmPassword={confirmPassword}", new StringContent(JsonConvert.SerializeObject(new { newPassword }), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendVerificationCode(string email)
        {
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/SendCode/SendVerificationCode", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<string> GetVerificationCodeFromRedisAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Email is empty in GetVerificationCodeFromRedisAsync.");
                return null;
            }
            var encodedEmail = Uri.EscapeDataString(email); // Mã hóa email
            var response = await _httpClient.GetAsync($"/api/SendCode/GetVerificationCode?email={encodedEmail}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error retrieving verification code: {response.StatusCode}, Details: {errorContent}");
            return null;
        }

    }
}
