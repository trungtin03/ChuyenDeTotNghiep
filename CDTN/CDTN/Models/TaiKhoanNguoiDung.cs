namespace CDTN.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TaiKhoanNguoiDung")]
    public partial class TaiKhoanNguoiDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaiKhoanNguoiDung()
        {
            HoaDon = new HashSet<HoaDon>();
            PhanHoiHoTro = new HashSet<PhanHoiHoTro>();
        }

        [Key]
        public int MaTK { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(11)]
        public string SoDienThoai { get; set; }

        [Required]
        [StringLength(100)]
        public string DiaChi { get; set; }

        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public bool GioiTinh { get; set; }

        public bool TrangThai { get; set; }

        [StringLength(255)]
        public string ResetPasswordToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhanHoiHoTro> PhanHoiHoTro { get; set; }
    }
}
