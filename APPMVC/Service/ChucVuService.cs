using AppData.Model;
using APPMVC.IService;
using System.Net.Http;

namespace APPMVC.Service
{
    public class ChucVuService : IChucVuService
    {
        HttpClient httpClient;
        public ChucVuService()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7198");
        }
        public Task CreateCV(ChucVu cv)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCV(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ChucVu>> GetAllChucVu()
        {
            var repo = await httpClient.GetFromJsonAsync<List<ChucVu>>("/api/ChucVu/GetAllChucVu");
            return repo;
        }

        public Task GetIdChucVu(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCV(ChucVu cv)
        {
            throw new NotImplementedException();
        }
    }
}
