using AppData.Model;
using APPMVC.IService;

namespace APPMVC.Service
{
    public class MauSacService : IMauSacService
    {
        HttpClient _httpClient;

        public MauSacService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7198");
        }

        public async Task Create(MauSac mauSac)
        {
            await _httpClient.PostAsJsonAsync("api/MauSac/them", mauSac);
        }

        public async Task Delete(Guid id)
        {
            await _httpClient.DeleteAsync($"api/MauSac/Xoa?id={id}");
        }

        public Task<List<MauSac>> GetMauSac(string? name)
        {
            var repo = _httpClient.GetFromJsonAsync<List<MauSac>>($"api/MauSac/getall?name={name}");
            return repo;
        }

        public Task<MauSac> GetMauSacById(Guid id)
        {
            var mauSac = _httpClient.GetFromJsonAsync<MauSac>($"api/MauSac/getbyid?id={id}");
            return mauSac;
        }

        public async Task Update(MauSac mauSac)
        {
            await _httpClient.PutAsJsonAsync("api/MauSac/Sua", mauSac);
        }
        public Task<List<MauSac>> GetMauSacBySanPhamId(Guid sanPhamId)
        {
            var mauSac = _httpClient.GetFromJsonAsync<List<MauSac>>($"api/MauSac/getmausacbyid?sanPhamId={sanPhamId}");
            return mauSac;
        }

        public async Task<List<MauSac>> GetMauSacByIdsAsync(List<Guid> mauSacIds)
        {
            if (mauSacIds == null || !mauSacIds.Any())
            {
                throw new ArgumentException("List of MauSac IDs cannot be null or empty.", nameof(mauSacIds));
            }

            // Join the IDs into a query string
            var idsQuery = string.Join(",", mauSacIds);
            return await _httpClient.GetFromJsonAsync<List<MauSac>>($"api/MauSac/getbyids?mauSacIds={idsQuery}");

        }
    }
}