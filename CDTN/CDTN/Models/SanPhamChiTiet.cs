namespace CDTN.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanPhamChiTiet")]
    public partial class SanPhamChiTiet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPhamChiTiet()
        {
            ChiTietHoaDon = new HashSet<ChiTietHoaDon>();
            NhapKho = new HashSet<NhapKho>();
        }

        [Key]
        public int IDCTSP { get; set; }

        public int MaSP { get; set; }

        public int MaKichCo { get; set; }

        public int SoLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDon { get; set; }

        public virtual KichCo KichCo { get; set; }

        public virtual SanPham SanPham { get; set; }
        public int? MaMau { get; set; }  

        [ForeignKey("MaMau")]
        public virtual MauSac MauSac { get; set; }
        public virtual ICollection<NhapKho> NhapKho { get; set; }
    }
}
