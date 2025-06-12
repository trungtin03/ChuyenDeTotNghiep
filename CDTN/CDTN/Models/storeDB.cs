using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CDTN.Models
{
    public partial class storeDB : DbContext
    {
        public storeDB()
            : base("name=storeDB")
        {       
        }

        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public virtual DbSet<DanhMuc> DanhMuc { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<KichCo> KichCo { get; set; }
        public virtual DbSet<PhanHoiHoTro> PhanHoiHoTro { get; set; }
        public virtual DbSet<SanPham> SanPham { get; set; }
        public virtual DbSet<SanPhamChiTiet> SanPhamChiTiet { get; set; }
        public virtual DbSet<TaiKhoanNguoiDung> TaiKhoanNguoiDung { get; set; }
        public virtual DbSet<TaiKhoanQuanTri> TaiKhoanQuanTri { get; set; }
        public virtual DbSet<ThongBao> ThongBao { get; set; }
        public virtual DbSet<ChucNang> ChucNang { get; set; }
        public virtual DbSet<Quyen> Quyen { get; set; }
        public virtual DbSet<PhanQuyen> PhanQuyen { get; set; }
        public DbSet<MauSac> MauSac { get; set; }
        public DbSet<SanPhamMau> SanPhamMau { get; set; }
        public virtual DbSet<NhapKho> NhapKho { get; set; }
        public DbSet<DanhGia> DanhGia { get; set; }
        public virtual DbSet<DiscountCode> DiscountCodes { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(e => e.GiaMua)
                .HasPrecision(19, 4);

            modelBuilder.Entity<HoaDon>()
                .Property(e => e.SoDienThoaiNhan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.Gia)
                .HasPrecision(19, 4);
            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.TenDangNhap)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.SoDienThoai)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.ResetPasswordToken)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanQuanTri>()
                .Property(e => e.TenDangNhap)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanQuanTri>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);
        }
    }
}
