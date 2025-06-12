using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    [Table("NhapKho")]
    public class NhapKho
    {
        [Key]
        public int MaNhap { get; set; }

        [Required]
        public int IDCTSP { get; set; }

        [Required]
        public int SoLuongNhap { get; set; }

        [Required]
        public DateTime NgayNhap { get; set; }

        [ForeignKey("IDCTSP")]
        public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }
    }
}
