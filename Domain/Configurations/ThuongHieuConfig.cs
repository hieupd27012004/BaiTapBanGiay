using AppData.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Configurations
{
	public class ThuongHieuConfig : IEntityTypeConfiguration<ThuongHieu>
	{
		public void Configure(EntityTypeBuilder<ThuongHieu> builder)
		{
			builder.HasKey(p => p.IdThuongHieu);
			builder.HasData(
                new ThuongHieu
                {
                    IdThuongHieu = Guid.NewGuid(),
                    TenThuongHieu = "Nike",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new ThuongHieu
                {
                    IdThuongHieu = Guid.NewGuid(),
                    TenThuongHieu = "Adidas",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new ThuongHieu
                {
                    IdThuongHieu = Guid.NewGuid(),
                    TenThuongHieu = "Puma",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                }
            );
        }
	}
}
