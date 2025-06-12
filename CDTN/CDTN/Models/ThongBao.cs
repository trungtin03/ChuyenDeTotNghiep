namespace CDTN.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThongBao")]
    public partial class ThongBao
    {
        public int ID { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public DateTime ThoiGian { get; set; }

        public bool DaXem { get; set; }

        [StringLength(100)]
        public string Loai { get; set; }
    }
}
