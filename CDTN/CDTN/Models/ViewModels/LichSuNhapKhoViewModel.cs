using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDTN.Models.ViewModels
{
    public class LichSuNhapKhoViewModel
    {
        public DateTime NgayNhap { get; set; }
        public string TenSanPham { get; set; }
        public string TenSize { get; set; }
        public string TenMau { get; set; }
        public int SoLuongNhap { get; set; }
    }
}