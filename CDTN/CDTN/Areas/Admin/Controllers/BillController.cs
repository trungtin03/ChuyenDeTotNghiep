using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using PagedList;
using ClosedXML.Excel;
using System.IO;
using CDTN.Helpers;
using System.Data.Entity;

namespace CDTN.Areas.Admin.Controllers
{
    public class BillController : BaseController
    {
        storeDB db = new storeDB();

        // GET: Admin/Bill
        [HttpGet]
        public ActionResult Index(DateTime? searchString, int? status, int page = 1, int pageSize = 10)
        {
            var tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
            if (!PhanQuyenHelper.CoQuyen(tk.ID, "Xem hóa đơn"))
            {
                return RedirectToAction("AccessDenied", "Home"); // hoặc View thông báo
            }

            List<HoaDon> hoaDons = db.HoaDon.Include("TaiKhoanNguoiDung").ToList();
            if (status != null)
            {
                hoaDons = hoaDons.Where(x => x.TrangThai == status).ToList();
                ViewBag.Status = status;
            }
            if (searchString != null)
            {
                ViewBag.searchString = searchString.Value.ToString("yyyy-MM-dd");
                string search = searchString.Value.ToString("dd/MM/yyyy");
                hoaDons = hoaDons.Where(hd => hd.NgayDat.ToString().Contains(search)).ToList();
            }
            return View(hoaDons.OrderBy(hd => hd.NgayDat).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult Index(int id)
        {
            HoaDon hd = db.HoaDon.Include("TaiKhoanNguoiDung")
                .FirstOrDefault(x => x.MaHD == id);

            IEnumerable<ChiTietHoaDon> chiTietHoaDons = db.ChiTietHoaDon
                .Include("SanPhamChiTiet")
                .Include("SanPhamChiTiet.KichCo")
                .Where(x => x.MaHD == id);

            List<SanPham> list = new List<SanPham>();
            foreach (ChiTietHoaDon item in chiTietHoaDons)
            {
                list.Add(db.SanPham.FirstOrDefault(x => x.MaSP == item.SanPhamChiTiet.MaSP));
            }

            return Json(new { hoadon = hd, cthd = chiTietHoaDons, sp = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
public JsonResult ChangeStatus(int mahd, int stt)
{
    var tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
    if (!PhanQuyenHelper.CoQuyen(tk.ID, "Sửa hóa đơn"))
    {
        return Json(new { status = false, message = "Không có quyền sửa hóa đơn." });
    }

    try
    {
        HoaDon hd = db.HoaDon.FirstOrDefault(x => x.MaHD == mahd);
        if (hd == null)
            return Json(new { status = false });

        // ✅ Nếu đơn bị hủy → hoàn lại hàng vào kho
        if (stt == 0 && hd.TrangThai != 0)
        {
            var chiTiet = db.ChiTietHoaDon.Where(c => c.MaHD == mahd).ToList();

            foreach (var item in chiTiet)
            {
                var spct = db.SanPhamChiTiet.FirstOrDefault(x => x.IDCTSP == item.IDCTSP);
                if (spct != null)
                {
                    spct.SoLuong += item.SoLuongMua;
                }
            }
        }

        hd.TrangThai = stt;
        hd.NguoiSua = tk.HoTen;
        hd.NgaySua = DateTime.Now;
        db.SaveChanges();

        db.ThongBao.Add(new ThongBao
        {
            NoiDung = $"Admin đã cập nhật trạng thái đơn hàng #{hd.MaHD} thành: {(stt == 0 ? "Đã hủy" : stt.ToString())}",
            ThoiGian = DateTime.Now,
            Loai = "hóa đơn",
            DaXem = false
        });
        db.SaveChanges();

        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
    }
    catch (Exception)
    {
        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
    }
}

        public ActionResult Details(int id)
        {
            var hoaDon = db.HoaDon
                .Include(h => h.TaiKhoanNguoiDung)
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.SanPham))
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.KichCo))
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.MauSac)) 
                .FirstOrDefault(h => h.MaHD == id);

            if (hoaDon == null)
                return HttpNotFound();

            return View(hoaDon);
        }
        public ActionResult ExportToExcel(int id)
        {
            var hoaDon = db.HoaDon.Include("TaiKhoanNguoiDung").FirstOrDefault(h => h.MaHD == id);
            if (hoaDon == null)
            {
                return HttpNotFound("Không tìm thấy hóa đơn.");
            }

            var chiTiet = db.ChiTietHoaDon
                .Include("SanPhamChiTiet")
                .Include("SanPhamChiTiet.KichCo")
                .Where(c => c.MaHD == id)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("HoaDon_" + hoaDon.MaHD);

                ws.Cell("A1").Value = "HÓA ĐƠN BÁN HÀNG";
                ws.Range("A1:E1").Merge().Style
                    .Font.SetBold().Font.FontSize = 16;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("A3").Value = $"Mã hóa đơn: HD{hoaDon.MaHD}";
                ws.Cell("A4").Value = $"Khách hàng: {hoaDon.TaiKhoanNguoiDung.HoTen}";
                ws.Cell("A5").Value = $"Ngày đặt: {hoaDon.NgayDat:dd/MM/yyyy HH:mm}";

                ws.Cell("A7").Value = "Sản phẩm";
                ws.Cell("B7").Value = "Kích cỡ";
                ws.Cell("C7").Value = "Số lượng";
                ws.Cell("D7").Value = "Đơn giá (VNĐ)";
                ws.Cell("E7").Value = "Thành tiền";

                ws.Range("A7:E7").Style.Font.Bold = true;
                ws.Range("A7:E7").Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 8;
                foreach (var item in chiTiet)
                {
                    var sanPham = db.SanPham.FirstOrDefault(sp => sp.MaSP == item.SanPhamChiTiet.MaSP);

                    ws.Cell(row, 1).Value = sanPham.TenSP;
                    ws.Cell(row, 2).Value = item.SanPhamChiTiet.KichCo.TenKichCo;
                    ws.Cell(row, 3).Value = item.SoLuongMua;
                    ws.Cell(row, 4).Value = item.GiaMua;
                    ws.Cell(row, 5).Value = item.SoLuongMua * item.GiaMua;

                    row++;
                }

                ws.Cell(row + 1, 4).Value = "Tổng cộng:";
                ws.Cell(row + 1, 5).FormulaA1 = $"=SUM(E8:E{row})";
                ws.Range(row + 1, 4, row + 1, 5).Style.Font.Bold = true;

                ws.Columns().AdjustToContents();
                ws.Range("A7:E" + (row)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:E" + (row)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                ws.Range($"D8:D{row}").Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                ws.Range($"E8:E{row}").Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                ws.Cell(row + 1, 5).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                ws.Range($"C8:C{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // ✅ Thêm thông báo xuất Excel
                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã xuất file Excel cho hóa đơn #{hoaDon.MaHD}",
                    ThoiGian = DateTime.Now,
                    Loai = "hóa đơn",
                    DaXem = false
                });
                db.SaveChanges();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"HoaDon_{hoaDon.MaHD}.xlsx");
                }
            }
        }
    }
}
