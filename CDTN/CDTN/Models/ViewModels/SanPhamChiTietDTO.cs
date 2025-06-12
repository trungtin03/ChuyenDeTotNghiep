using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDTN.Models.ViewModels
{
    public class SanPhamChiTietDTO
    {
        public int IDCTSP { get; set; }
        public int MaSP { get; set; }
        public int MaMau { get; set; }
        public string TenKichCo { get; set; }
        public int SoLuong { get; set; }
    }

}