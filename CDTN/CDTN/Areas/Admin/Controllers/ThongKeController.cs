using CDTN.Models;
using CDTN.Models.ViewModels;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CDTN.Areas.Admin.Controllers
{
    public class ThongKeController : BaseController
    {
        private storeDB db = new storeDB();

        public ActionResult Index(int? year, int? categoryId, string keyword, string tab = "tab1",DateTime? fromDate = null, DateTime? toDate = null)
        {
            ViewBag.SelectedYear = year ?? DateTime.Now.Year;
            ViewBag.ActiveTab = tab;
            ViewBag.CategoryId = categoryId;
            ViewBag.Keyword = keyword;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd") ?? "";
            var hoaDonRaw = db.HoaDon
                .Include(h => h.TaiKhoanNguoiDung)
                .Include(h => h.ChiTietHoaDon)
                .ToList(); 
            ViewBag.TongTatCa = hoaDonRaw.Count;
            if (fromDate.HasValue)
                hoaDonRaw = hoaDonRaw.Where(h => h.NgayDat.Date >= fromDate.Value.Date).ToList();

            if (toDate.HasValue)
                hoaDonRaw = hoaDonRaw.Where(h => h.NgayDat.Date <= toDate.Value.Date).ToList();

            ViewBag.TrongNgay = hoaDonRaw.Count;

            var donThanhToan = hoaDonRaw.Where(h => h.TrangThai == 3).ToList();
            ViewBag.DaThanhToan = donThanhToan.Count;

            var donHangList = donThanhToan
                .OrderByDescending(h => h.NgayDat)
                .Select(h => new DonHangViewModel
                {
                    NgayDat = h.NgayDat,
                    MaDonHang = h.MaHD,
                    TenKhachHang = h.TaiKhoanNguoiDung?.HoTen ?? "(ẩn)",
                    TongTien = h.ChiTietHoaDon.Sum(ct => (decimal?)ct.GiaMua * ct.SoLuongMua) ?? 0,
                    TrangThai = "Đã thanh toán"
                })
                .ToList();
            ViewBag.DonHangTheoNgay = donHangList;
            ViewBag.DoanhThu = donHangList.Sum(x => x.TongTien);
            ViewBag.SoDon = donHangList.Count;


            // Danh mục
            var danhMucs = db.DanhMuc.ToList();
            ViewBag.DanhMucList = new SelectList(danhMucs, "MaDM", "TenDanhMuc", categoryId);

            // Truy vấn kho
            var query = db.SanPhamChiTiet
                .Include(sp => sp.SanPham)
                .Include(sp => sp.MauSac)
                .Include(sp => sp.KichCo)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(sp => sp.SanPham.MaDM == categoryId.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(sp => sp.SanPham.TenSP.Contains(keyword));

            var thongKeKho = query
                .Select(ctsp => new ThongKeViewModel
                {
                    TenSanPham = ctsp.SanPham.TenSP,
                    Mau = ctsp.MauSac.TenMau,
                    Size = ctsp.KichCo.TenKichCo,
                    TongNhap = db.NhapKho
                        .Where(nk => nk.IDCTSP == ctsp.IDCTSP)
                        .Sum(nk => (int?)nk.SoLuongNhap) ?? 0,
                    TongBan = db.ChiTietHoaDon
                        .Where(ct => ct.IDCTSP == ctsp.IDCTSP)
                        .Sum(ct => (int?)ct.SoLuongMua) ?? 0,
                    TonKho = ctsp.SoLuong
                })
                .ToList();

            var lichSuNhap = db.NhapKho
    .Include(n => n.SanPhamChiTiet)
    .Include(n => n.SanPhamChiTiet.SanPham)
    .Include(n => n.SanPhamChiTiet.KichCo)
    .Include(n => n.SanPhamChiTiet.MauSac)
    .OrderByDescending(n => n.NgayNhap)
    .Take(100)
    .ToList()
    .Select(n => new LichSuNhapKhoViewModel
    {
        NgayNhap = n.NgayNhap,
        TenSanPham = n.SanPhamChiTiet?.SanPham?.TenSP,
        TenSize = n.SanPhamChiTiet?.KichCo?.TenKichCo,
        TenMau = n.SanPhamChiTiet?.MauSac?.TenMau,
        SoLuongNhap = n.SoLuongNhap
    })
    .ToList();
            ViewBag.LichSuNhapKho = lichSuNhap;
            return View(thongKeKho);
        }
        [HttpGet]
        public JsonResult ChartData(DateTime? from, DateTime? to)
        {
            if (!from.HasValue || !to.HasValue || from > to)
            {
                return Json(new { success = false, message = "Khoảng thời gian không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            var data = db.HoaDon
                .Where(h => h.NgayDat >= from && h.NgayDat <= to && h.TrangThai == 3)
                .GroupBy(h => DbFunctions.TruncateTime(h.NgayDat))
                .Select(g => new
                {
                    Ngay = g.Key.Value,
                    TongTien = g.Sum(x => x.ChiTietHoaDon.Sum(ct => ct.GiaMua * ct.SoLuongMua) - (x.DiscountAmount ?? 0))
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            var labels = data.Select(d => d.Ngay.ToString("dd/MM/yyyy")).ToList();
            var values = data.Select(d => d.TongTien).ToList();

            return Json(new
            {
                success = true,
                labels = labels,
                values = values
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportDoanhSoExcel(DateTime? from, DateTime? to)
        {
            if (!from.HasValue || !to.HasValue || from > to)
                return Content("Khoảng thời gian không hợp lệ.");

            // Lấy dữ liệu đơn hàng đã thanh toán theo ngày
            var chiTiet = db.ChiTietHoaDon
                .Include(ct => ct.SanPhamChiTiet.SanPham)
                .Include(ct => ct.SanPhamChiTiet.MauSac)
                .Include(ct => ct.HoaDon)
                .Where(ct => ct.HoaDon.TrangThai == 3 &&
                             DbFunctions.TruncateTime(ct.HoaDon.NgayDat) >= DbFunctions.TruncateTime(from) &&
                             DbFunctions.TruncateTime(ct.HoaDon.NgayDat) <= DbFunctions.TruncateTime(to))
                .ToList();

            // Gom nhóm theo sản phẩm + màu
            var group = chiTiet
                .GroupBy(ct => new
                {
                    TenSP = ct.SanPhamChiTiet.SanPham.TenSP,
                    Mau = ct.SanPhamChiTiet.MauSac.TenMau
                })
                .Select(g => new
                {
                    TenSanPham = g.Key.TenSP,
                    MauSac = g.Key.Mau,
                    SoLuongBan = g.Sum(x => x.SoLuongMua),
                    TongTien = g.Sum(x => x.SoLuongMua * x.GiaMua)
                })
                .ToList();

            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("DoanhSo");

                ws.Cell("A1").Value = $"THỐNG KÊ DOANH SỐ TỪ {from:dd/MM/yyyy} ĐẾN {to:dd/MM/yyyy}";
                ws.Range("A1:D1").Merge().Style
                    .Font.SetBold().Font.FontSize = 14;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("A2").Value = "Tên sản phẩm";
                ws.Cell("B2").Value = "Màu sắc";
                ws.Cell("C2").Value = "Số lượng bán";
                ws.Cell("D2").Value = "Tổng tiền (₫)";

                ws.Range("A2:D2").Style.Font.Bold = true;
                ws.Range("A2:D2").Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 3;
                foreach (var item in group)
                {
                    ws.Cell(row, 1).Value = item.TenSanPham;
                    ws.Cell(row, 2).Value = item.MauSac;
                    ws.Cell(row, 3).Value = item.SoLuongBan;
                    ws.Cell(row, 4).Value = item.TongTien;
                    row++;
                }

                // Tổng cuối bảng
                ws.Cell(row, 2).Value = "Tổng cộng:";
                ws.Cell(row, 3).FormulaA1 = $"=SUM(C3:C{row - 1})";
                ws.Cell(row, 4).FormulaA1 = $"=SUM(D3:D{row - 1})";
                ws.Range(row, 2, row, 4).Style.Font.Bold = true;

                ws.Range("C3:C" + (row)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("D3:D" + (row)).Style.NumberFormat.Format = "#,##0 \"₫\"";

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"DoanhSo_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
                }
            }
        }
    }
}
