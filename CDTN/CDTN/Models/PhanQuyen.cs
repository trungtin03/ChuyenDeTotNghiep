using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CDTN.Models
{
    [Table("PhanQuyen")]
    public class PhanQuyen
    {
        [Key]
        public int ID { get; set; }

        public int IDTaiKhoan { get; set; }
        public int MaQuyen { get; set; }

        [ForeignKey("IDTaiKhoan")]
        public virtual TaiKhoanQuanTri TaiKhoanQuanTri { get; set; }

        [ForeignKey("MaQuyen")]
        public virtual Quyen Quyen { get; set; }
    }

}