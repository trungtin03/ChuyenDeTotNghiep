using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using System.Data.Entity;
using CDTN.Models.ViewModels;

namespace CDTN.Controllers
{
    public class BillController : Controller
    {
        storeDB db = new storeDB();

        // GET: Bill
        [HttpGet]
        public ActionResult ListBills()
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (tk == null)
                return RedirectToAction("Login", "Home");

            var list = db.HoaDon.Where(p => p.MaTK == tk.MaTK);

            if (!string.IsNullOrEmpty(Request.QueryString["status"]))
            {
                if (int.TryParse(Request.QueryString["status"], out int stt))
                {
                    list = list.Where(p => p.TrangThai == stt);
                }
            }
            var result = list.OrderByDescending(x => x.NgayDat).ToList();
            return View(result);
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (tk == null)
                return RedirectToAction("Login", "Home");

            var hoaDon = db.HoaDon
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.SanPham))
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.KichCo))
                .Include(h => h.ChiTietHoaDon.Select(ct => ct.SanPhamChiTiet.MauSac))
                .FirstOrDefault(h => h.MaHD == id && h.MaTK == tk.MaTK);

            if (hoaDon == null)
                return RedirectToAction("PageNotFound", "Error");

            return View(hoaDon);
        }


        [HttpPost]
        public JsonResult CreateBill(FormCollection form)
        {
            try
            {
                HoaDon hd = new HoaDon
                {
                    MaTK = int.Parse(form["MaTK"]),
                    HoTenNguoiNhan = form["HoTenNguoiNhan"],
                    SoDienThoaiNhan = form["SoDienThoaiNhan"],
                    DiaChiNhan = form["DiaChiNhan"],
                    GhiChu = form["GhiChu"],
                    NgayDat = DateTime.Now,
                    NgaySua = DateTime.Now,
                    TrangThai = 1,
                    DiscountCodeUsed = form["DiscountCodeUsed"],
                    DiscountAmount = string.IsNullOrEmpty(form["DiscountAmount"]) ? (decimal?)null : Convert.ToDecimal(form["DiscountAmount"]),
                };

                db.HoaDon.Add(hd);
                db.SaveChanges();

                var list = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];

                foreach (var item in list)
                {
                    item.MaHD = hd.MaHD;
                    db.ChiTietHoaDon.Add(item);
                }

                // ✅ Trừ tồn kho
                foreach (var item in list)
                {
                    var ctsp = db.SanPhamChiTiet.FirstOrDefault(x => x.IDCTSP == item.IDCTSP);
                    if (ctsp != null)
                    {
                        ctsp.SoLuong -= item.SoLuongMua;
                        if (ctsp.SoLuong < 0) ctsp.SoLuong = 0;
                    }
                }

                db.SaveChanges();
                Session.Remove(CDTN.Session.ConstainCart.CART);

                return Json(new { status = true, billid = hd.MaHD });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ChangeStatus(int mahd, int stt)
        {
            try
            {
                TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
                HoaDon hd = db.HoaDon.FirstOrDefault(x => x.MaHD == mahd && x.MaTK == tk.MaTK);

                if (hd == null || hd.TrangThai != 1)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                if (stt == 0)
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

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
