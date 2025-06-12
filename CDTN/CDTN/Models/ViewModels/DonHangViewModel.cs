using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDTN.Models.ViewModels
{
    public class DonHangViewModel
    {
        public DateTime NgayDat { get; set; }
        public int MaDonHang { get; set; }
        public string TenKhachHang { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }
}