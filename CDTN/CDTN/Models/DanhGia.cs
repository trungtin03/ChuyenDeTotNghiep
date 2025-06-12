using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        public int MaSP { get; set; }

        [Required]
        public int MaTK { get; set; }

        [Range(1, 5)]
        public int SoSao { get; set; }

        [Required]
        [StringLength(1000)]
        public string BinhLuan { get; set; }

        public DateTime NgayDanhGia { get; set; }

        [ForeignKey("MaSP")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("MaTK")]
        public virtual TaiKhoanNguoiDung TaiKhoan { get; set; }
    }
}
