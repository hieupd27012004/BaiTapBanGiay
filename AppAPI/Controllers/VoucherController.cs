using AppAPI.IService;
using AppData.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : Controller
    {
        private readonly IVoucherService _service;
        private readonly ILogger<VoucherController> _logger;
        public VoucherController(IVoucherService service, ILogger<VoucherController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("GetAllVouchers")]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _service.GetVouchersAsync();
            return Ok(vouchers);
        }

        [HttpPost("CreateVoucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            try
            {
                _logger.LogInformation($"Received create request for voucher: {JsonSerializer.Serialize(voucher)}");

                // Đảm bảo ID mới
                voucher.VoucherId = Guid.NewGuid();

                // Set các giá trị mặc định
                voucher.NgayTao = DateTime.Now;
                if (string.IsNullOrEmpty(voucher.NguoiTao))
                    voucher.NguoiTao = "Admin";
                voucher.SoLuongVoucherConLai = voucher.TongSoLuongVoucher;

                var result = await _service.CreateAsync(voucher);
                if (result)
                {
                    _logger.LogInformation($"Successfully created voucher with ID: {voucher.VoucherId}");
                    return Ok(voucher);
                }

                _logger.LogWarning("Failed to create voucher in service layer");
                return BadRequest("Failed to create voucher");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating voucher");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("UpdateVoucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] Voucher voucher)
        {
            try
            {
                if (id != voucher.VoucherId)
                {
                    return BadRequest("Id mismatch");
                }

                var existingVoucher = await _service.GetVoucherByIdAsync(id);
                if (existingVoucher == null)
                {
                    return NotFound("Voucher not found");
                }

                // Preserve original creation info
                voucher.NgayTao = existingVoucher.NgayTao;
                voucher.NguoiTao = existingVoucher.NguoiTao;

                // Set update info
                voucher.NgayUpdate = DateTime.Now;
                voucher.NguoiUpdate = "Admin";

                var result = await _service.UpdateAsync(voucher);
                if (result)
                {
                    return Ok(voucher);
                }
                return BadRequest("Failed to update voucher");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteVoucher")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var voucher = await _service.GetVoucherByIdAsync(id);
            if(voucher == null)
            {
                return NotFound("Voucher not found");
            }
            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error while deleting the Voucher");
            }
            return Ok("Voucher deleted successfully");
        }

        [HttpGet("GetVoucherById")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _service.GetVoucherByIdAsync(id);
            if(voucher == null)
            {
                return NotFound("Voucher not found.");
            }
            return Ok(voucher);
        }
    }
}
