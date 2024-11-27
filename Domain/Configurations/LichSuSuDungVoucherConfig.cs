using AppData.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppData.Configurations
{
    public class LichSuSuDungVoucherConfig : IEntityTypeConfiguration<LichSuSuDungVoucher>
    {
        public void Configure(EntityTypeBuilder<LichSuSuDungVoucher> builder)
        {
            builder.HasKey(lsv => new { lsv.IdVoucher, lsv.IdKhachHang });
            builder.HasIndex(p => new { p.IdVoucher, p.IdKhachHang, p.IdOrder })
            .IsUnique();

            builder.HasOne(lsv => lsv.Voucher)
                .WithMany(v => v.LichSuSuDungVouchers)
                .HasForeignKey(lsv => lsv.IdVoucher)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lsv => lsv.KhachHang)
                .WithMany(kh => kh.LichSuSuDungVouchers)
                .HasForeignKey(lsv => lsv.IdKhachHang)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
