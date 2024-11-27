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
	public class MauSacConfig : IEntityTypeConfiguration<MauSac>
	{
		public void Configure(EntityTypeBuilder<MauSac> builder)
		{
			builder.HasKey(p => p.IdMauSac);
            builder.HasData(
                      new MauSac
                      {
                          IdMauSac = Guid.NewGuid(),
                          TenMauSac = "Red",
                          NgayCapNhat = DateTime.Now,
                          NgayTao = DateTime.Now,
                          NguoiCapNhat = "Admin",
                          NguoiTao = "Admin",
                          KichHoat = 1
                      },
                      new MauSac
                      {
                          IdMauSac = Guid.NewGuid(),
                          TenMauSac = "Green",
                          NgayCapNhat = DateTime.Now,
                          NgayTao = DateTime.Now,
                          NguoiCapNhat = "Admin",
                          NguoiTao = "Admin",
                          KichHoat = 1
                      },
                      new MauSac
                      {
                          IdMauSac = Guid.NewGuid(),
                          TenMauSac = "Blue",
                          NgayCapNhat = DateTime.Now,
                          NgayTao = DateTime.Now,
                          NguoiCapNhat = "Admin",
                          NguoiTao = "Admin",
                          KichHoat = 1
                      }
                  );
        }
	}
}
