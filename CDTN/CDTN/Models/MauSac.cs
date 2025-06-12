using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    [Table("MauSac")]
    public class MauSac
    {
        [Key]
        public int MaMau { get; set; }

        [Required]
        [StringLength(50)]
        public string TenMau { get; set; }

        [Required]
        [StringLength(10)]
        public string MaMauHex { get; set; }

        // Liên kết
        public virtual ICollection<SanPhamMau> SanPhamMaus { get; set; }
        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }
    }
}
