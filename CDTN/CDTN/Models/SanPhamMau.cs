using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    [Table("SanPhamMau")]
    public class SanPhamMau
    {
        [Key]
        public int ID { get; set; }

        public int MaSP { get; set; }
        public int MaMau { get; set; }
        public int SoLuong { get; set; }

        // Navigation
        [ForeignKey("MaSP")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("MaMau")]
        public virtual MauSac MauSac { get; set; }
    }
}