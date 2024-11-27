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
	public class KieuDangConfig : IEntityTypeConfiguration<KieuDang>
	{
		public void Configure(EntityTypeBuilder<KieuDang> builder)
		{
			builder.HasKey(p => p.IdKieuDang);
			builder.HasData(
                new KieuDang
                {
                    IdKieuDang = Guid.NewGuid(),
                    TenKieuDang = "Thể Thao",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new KieuDang
                {
                    IdKieuDang = Guid.NewGuid(),
                    TenKieuDang = "Cổ Điển",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new KieuDang
                {
                    IdKieuDang = Guid.NewGuid(),
                    TenKieuDang = "Hiện Đại",
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
