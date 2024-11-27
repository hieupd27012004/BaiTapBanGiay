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
	public class DeGiayConfig : IEntityTypeConfiguration<DeGiay>
	{
		public void Configure(EntityTypeBuilder<DeGiay> builder)
		{
			builder.HasKey(p => p.IdDeGiay);
            builder.HasData(
                new DeGiay
                {
                    IdDeGiay = Guid.NewGuid(),
                    TenDeGiay = "Đế cao su",
                    NgayCapNhat = new DateTime(2023, 10, 22),
                    NgayTao = new DateTime(2023, 10, 22),
                    NguoiCapNhat = "Admin",
                    NguoiTao = "Admin",
                    KichHoat = 1
                },
                new DeGiay
                {
                    IdDeGiay = Guid.NewGuid(),
                    TenDeGiay = "Đế nhựa",
                    NgayCapNhat = new DateTime(2023, 10, 22),
                    NgayTao = new DateTime(2023, 10, 22),
                    NguoiCapNhat = "Admin",
                    NguoiTao = "Admin",
                    KichHoat = 1
                },
                new DeGiay
                {
                    IdDeGiay = Guid.NewGuid(),
                    TenDeGiay = "Đế vải",
                    NgayCapNhat = new DateTime(2023, 10, 22),
                    NgayTao = new DateTime(2023, 10, 22),
                    NguoiCapNhat = "Admin",
                    NguoiTao = "Admin",
                    KichHoat = 1
                }
            );
        }

	}
}
