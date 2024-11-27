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
	public class DanhMucConfig : IEntityTypeConfiguration<DanhMuc>
	{
		public void Configure(EntityTypeBuilder<DanhMuc> builder)
		{
			builder.HasKey(p => p.IdDanhMuc);
			builder.HasData(
                new DanhMuc
                {
                    IdDanhMuc = Guid.NewGuid(),
                    TenDanhMuc = "Giày Thể Thao",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new DanhMuc
                {
                    IdDanhMuc = Guid.NewGuid(),
                    TenDanhMuc = "Giày Da",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Admin",
                    NguoiCapNhat = "Admin",
                    KichHoat = 1
                },
                new DanhMuc
                {
                    IdDanhMuc = Guid.NewGuid(),
                    TenDanhMuc = "Giày Cao Gót",
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
