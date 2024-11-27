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
	public class ChatLieuConfig : IEntityTypeConfiguration<ChatLieu>
	{
		public void Configure(EntityTypeBuilder<ChatLieu> builder)
		{
			builder.HasKey(p => p.IdChatLieu);
            builder.HasData(
              new ChatLieu
              {
                  IdChatLieu = Guid.NewGuid(),
                  TenChatLieu = "Vải Cotton",
                  NgayCapNhat = DateTime.Now,
                  NgayTao = DateTime.Now,
                  NguoiCapNhat = "Admin",
                  NguoiTao = "Admin",
                  KichHoat = 1
              },
              new ChatLieu
              {
                  IdChatLieu = Guid.NewGuid(),
                  TenChatLieu = "Da thật",
                  NgayCapNhat = DateTime.Now,
                  NgayTao = DateTime.Now,
                  NguoiCapNhat = "Admin",
                  NguoiTao = "Admin",
                  KichHoat = 1
              },
              new ChatLieu
              {
                  IdChatLieu = Guid.NewGuid(),
                  TenChatLieu = "Vải Polyester",
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
