using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    [Table("TaiKhoanQuanTri")]
    public partial class TaiKhoanQuanTri
    {
        public TaiKhoanQuanTri()
        {
            PhanQuyens = new HashSet<PhanQuyen>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; }

        public bool LoaiTaiKhoan { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<PhanQuyen> PhanQuyens { get; set; }
    }
}
