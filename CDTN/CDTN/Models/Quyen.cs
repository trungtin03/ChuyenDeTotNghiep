using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CDTN.Models
{
    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        public int MaQuyen { get; set; }

        [Required]
        [StringLength(100)]
        public string TenQuyen { get; set; }

        public int MaCN { get; set; }

        [ForeignKey("MaCN")]
        public virtual ChucNang ChucNang { get; set; }

        public virtual ICollection<PhanQuyen> PhanQuyen { get; set; }
    }


}