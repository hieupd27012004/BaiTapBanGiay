﻿using AppData.Model;
using APPMVC.IService;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AppAPI.Service
{
    public class SanPhamChiTietMauSacService : ISanPhamChiTietMauSacService
    {
        private readonly HttpClient _httpClient;

        public SanPhamChiTietMauSacService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7198")
            };
        }

        // Retrieve all Mau Sac items
        public async Task<List<SanPhamChiTietMauSac>> GetAllMauSac()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<SanPhamChiTietMauSac>>("api/SanPhamChiTietMauSac");
            }
            catch (HttpRequestException ex)
            {
                // Handle error here
                Console.WriteLine($"Error fetching Mau Sac: {ex.Message}");
                return new List<SanPhamChiTietMauSac>(); // Return an empty list or handle accordingly
            }
        }

        // Retrieve a specific Mau Sac by ID
        public async Task<SanPhamChiTietMauSac> GetMauSacById(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<SanPhamChiTietMauSac>($"api/SanPhamChiTietMauSac/getbyid?id={id}");
        }

        // Create a new Mau Sac
        public async Task Create(SanPhamChiTietMauSac sanPhamChiTietMauSac)
        {
            await _httpClient.PostAsJsonAsync("api/SanPhamChiTietMauSac/them", sanPhamChiTietMauSac);
        }

        // Update an existing Mau Sac
        public async Task Update(SanPhamChiTietMauSac sanPhamChiTietMauSac)
        {
            await _httpClient.PutAsJsonAsync("api/SanPhamChiTietMauSac/Sua", sanPhamChiTietMauSac);
        }

        // Delete a Mau Sac by ID
        public async Task Delete(Guid id)
        {
            await _httpClient.DeleteAsync($"api/SanPhamChiTietMauSac/Xoa?id={id}");
        }

        public async Task<List<MauSac>> GetMauSacIdsBySanPhamChiTietId(Guid sanPhamChiTietId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<MauSac>>($"api/SanPhamChiTietMauSac/mauSacIds?id={sanPhamChiTietId}");
            }
            catch (HttpRequestException ex)
            {
                // Handle error here
                Console.WriteLine($"Error fetching Mau Sac IDs: {ex.Message}");
                return new List<MauSac>(); // Return an empty list or handle accordingly
            }
        }
    }
}