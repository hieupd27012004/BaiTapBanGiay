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
	public class ChucVuConfig : IEntityTypeConfiguration<ChucVu>
	{
		public void Configure(EntityTypeBuilder<ChucVu> builder)
		{
			builder.HasKey(p => p.IdChucVu);
            builder.HasData(
              new ChucVu { IdChucVu = Guid.NewGuid(), Code = "QL", TenChucVu = "Quản lý" },
              new ChucVu { IdChucVu = Guid.NewGuid(), Code = "NV", TenChucVu = "Nhân viên" },
              new ChucVu { IdChucVu = Guid.NewGuid(), Code = "KT", TenChucVu = "Kế toán" },
              new ChucVu { IdChucVu = Guid.NewGuid(), Code = "KK", TenChucVu = "Thủ kho" }
          );
        }
	}
}
