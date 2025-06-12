using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDTN.Models
{
    public enum DiscountType
    {
        OrderTotal = 0, 
        SpecificProduct = 1 
    }

    [Table("DiscountCodes")]
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; } 

        public bool IsPercentage { get; set; } 

        [Required]
        public DiscountType Type { get; set; } 

        public int? ProductId { get; set; } 

        [ForeignKey("ProductId")]
        public virtual SanPham Product { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
