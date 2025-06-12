using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CDTN.Models
{
    [Table("ChucNang")]
    public class ChucNang
    {
        [Key]
        public int MaCN { get; set; }

        [Required]
        [StringLength(100)]
        public string TenChucNang { get; set; }

        [StringLength(255)]
        public string MoTa { get; set; }

        public virtual ICollection<Quyen> Quyen { get; set; }
    }

}