namespace CDTN.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhanHoiHoTro")]
    public partial class PhanHoiHoTro
    {
        public int ID { get; set; }

        public int? MaTK { get; set; }

        [Required]
        [StringLength(150)]
        public string Subject { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Message { get; set; }

        public DateTime NgayGui { get; set; }

        public int TrangThai { get; set; }

        [Column(TypeName = "ntext")]
        public string PhanHoi { get; set; }

        public virtual TaiKhoanNguoiDung TaiKhoanNguoiDung { get; set; }
    }
}
