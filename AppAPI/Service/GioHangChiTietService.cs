﻿using AppAPI.IRepository;
using AppAPI.IService;
using AppData.Model;

namespace AppAPI.Service
{
	public class GioHangChiTietService : IGioHangChiTietService
	{
		private readonly IGioHangChiTietRepo _repository;

		public GioHangChiTietService(IGioHangChiTietRepo repository)
		{
			_repository = repository;
		}
		public async Task<List<GioHangChiTiet>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<GioHangChiTiet> GetByIdAsync(Guid id)
		{
			return await _repository.GetByIdAsync(id);
		}

		public async Task AddAsync(GioHangChiTiet gioHangChiTiet)
		{
			await _repository.AddAsync(gioHangChiTiet);
		}

		public async Task UpdateAsync(GioHangChiTiet gioHangChiTiet)
		{
			await _repository.UpdateAsync(gioHangChiTiet);
		}

		public async Task DeleteAsync(Guid id)
		{
			await _repository.DeleteAsync(id);
		}

        public async Task<List<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId)
        {
            return await _repository.GetByGioHangIdAsync(gioHangId);
        }

        public async Task ClearCartByIdAsync(Guid cartId)
        {
            var cartItems = await _repository.GetByGioHangIdAsync(cartId); 

            if (cartItems != null && cartItems.Count > 0)
            {
                foreach (var item in cartItems)
                {
                    await _repository.DeleteAsync(item.IdGioHangChiTiet); 
                }
            }
            else
            {
                throw new InvalidOperationException("Giỏ hàng không tồn tại hoặc đã rỗng."); 
            }
        }

        public async Task<double> GetTotalQuantityBySanPhamChiTietIdAsync(Guid sanPhamChiTietId, Guid cartId)
        {
            var totalQuantity = await _repository.GetTotalQuantityBySanPhamChiTietIdAsync(sanPhamChiTietId, cartId);
            return totalQuantity;
        }
    }
}
