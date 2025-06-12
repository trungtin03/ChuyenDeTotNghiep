using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDTN.Models.ViewModels
{
    public class CreateBillViewModel
    {
        public int MaTK { get; set; }
        public string HoTenNguoiNhan { get; set; }
        public string SoDienThoaiNhan { get; set; }
        public string DiaChiNhan { get; set; }
        public string GhiChu { get; set; }
    }
}