using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using PagedList;
using System.Data.Entity;
using CDTN.Models.ViewModels;
using Newtonsoft.Json;

namespace CDTN.Controllers
{
    public class ProductController : Controller
    {
        storeDB db = new storeDB();

        // GET: Product
        public ActionResult Shop(string searchString, int? madm, int page = 1, int pageSize = 9)
        {
            ViewBag.searchString = searchString;
            ViewBag.madm = madm;
            var sanphams = db.SanPham.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(sp => sp.TenSP.Contains(searchString));
            }

            if (madm != null && madm != 0)
            {
                sanphams = sanphams.Where(s => s.MaDM == madm);
                ViewBag.DanhMuc = db.DanhMuc.FirstOrDefault(d => d.MaDM == madm);
            }

            return View(sanphams.OrderBy(sp => sp.MaSP).ToPagedList(page, pageSize));
        }
        public ActionResult ProductDetail(int id)
        {
            var sp = db.SanPham
                .Include(s => s.DanhMuc)
                .FirstOrDefault(s => s.MaSP == id);

            var list = db.SanPhamChiTiet
                .Include(ct => ct.KichCo)
                .Include(ct => ct.MauSac)
                .Where(ct => ct.MaSP == id)
                .ToList();

            // Convert sang DTO để tránh vòng lặp khi serialize
            var spctDtos = list.Select(x => new SanPhamChiTietDTO
            {
                IDCTSP = x.IDCTSP,
                MaSP = x.MaSP,
                MaMau = x.MaMau ?? 0,
                TenKichCo = x.KichCo.TenKichCo,
                SoLuong = x.SoLuong
            }).ToList();

            // Serialize DTO sang JSON
            ViewBag.SPCT_JSON = JsonConvert.SerializeObject(spctDtos);

            var mauSacs = list
                .Where(ct => ct.MauSac != null)
                .Select(ct => ct.MauSac)
                .Distinct()
                .ToList();

            ViewBag.AllSizes = db.KichCo.ToList();
            ViewBag.MauSacs = mauSacs;
            ViewBag.SPCT = list; // vẫn truyền nếu view dùng
            ViewBag.Exitst = list.FirstOrDefault();

            var user = Session[CDTN.Session.ConstaintUser.USER_SESSION] as TaiKhoanNguoiDung;
            bool daMua = false;

            if (user != null)
            {
                var hasProduct = (from h in db.HoaDon
                                  join ct in db.ChiTietHoaDon on h.MaHD equals ct.MaHD
                                  join spct in db.SanPhamChiTiet on ct.IDCTSP equals spct.IDCTSP
                                  where h.MaTK == user.MaTK && h.TrangThai == 3 && spct.MaSP == id
                                  select h.MaHD).Any();

                daMua = hasProduct;
            }

            ViewBag.DaMua = daMua;

            // Danh sách đánh giá
            ViewBag.DanhGiaList = db.DanhGia
                .Include(d => d.TaiKhoan)
                .Where(d => d.MaSP == id)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToList();
            if (user != null)
            {
                var debugList = (from h in db.HoaDon
                                 join ct in db.ChiTietHoaDon on h.MaHD equals ct.MaHD
                                 join spct in db.SanPhamChiTiet on ct.IDCTSP equals spct.IDCTSP
                                 where h.MaTK == user.MaTK
                                 select new DebugReviewItem
                                 {
                                     MaHD = h.MaHD,
                                     TrangThai = h.TrangThai,
                                     MaSP = spct.MaSP
                                 }).ToList();

                ViewBag.DebugDanhGia = debugList;
            }
            return View(sp);
        }

        [HttpPost]
        public JsonResult Index(int id)
        {
            var sp = db.SanPham
                .Include(s => s.DanhMuc)
                .Include(s => s.SanPhamChiTiet.Select(ct => ct.KichCo))
                .FirstOrDefault(s => s.MaSP == id);

            if (sp == null)
            {
                return Json(new { status = false, message = "Không tìm thấy sản phẩm!" }, JsonRequestBehavior.AllowGet);
            }

            // ✅ Lấy danh sách màu
            var listMau = db.SanPhamMau
                .Where(m => m.MaSP == id)
                .Select(m => new
                {
                    MaMau = m.MaMau,
                    TenMau = m.MauSac.TenMau,
                    MaMauHex = m.MauSac.MaMauHex
                }).ToList();

            var result = new
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                Gia = sp.Gia,
                HinhAnh = sp.HinhAnh,
                MoTa = sp.MoTa,
                HuongDan = sp.HuongDan,
                ChatLieu = sp.ChatLieu,
                DanhMuc = new { TenDanhMuc = sp.DanhMuc.TenDanhMuc },
                SanPhamChiTiets = sp.SanPhamChiTiet.Select(ct => new
                {
                    IDCTSP = ct.IDCTSP,
                    MaKichCo = ct.MaKichCo,
                    SoLuong = ct.SoLuong,
                    KichCo = new { TenKichCo = ct.KichCo.TenKichCo }
                }),
                ListMau = listMau
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Detail(int id)
        {
            var spct = db.SanPhamChiTiet.FirstOrDefault(sp => sp.IDCTSP == id);

            if (spct == null)
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);

            return Json(spct, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(DanhGia danhGia)
        {
            var user = Session[CDTN.Session.ConstaintUser.USER_SESSION] as TaiKhoanNguoiDung;
            if (user == null)
                return RedirectToAction("Login", "Home");

            bool daMua = (from h in db.HoaDon
                          join ct in db.ChiTietHoaDon on h.MaHD equals ct.MaHD
                          join spct in db.SanPhamChiTiet on ct.IDCTSP equals spct.IDCTSP
                          where h.MaTK == user.MaTK && h.TrangThai == 3 && spct.MaSP == danhGia.MaSP
                          select h.MaHD).Any();

            if (!daMua)
            {
                TempData["ReviewError"] = "Bạn cần mua sản phẩm này trước khi đánh giá.";
                return RedirectToAction("ProductDetail", new { id = danhGia.MaSP });
            }

            bool daDanhGia = db.DanhGia.Any(d => d.MaSP == danhGia.MaSP && d.MaTK == user.MaTK);
            if (daDanhGia)
            {
                TempData["ReviewError"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("ProductDetail", new { id = danhGia.MaSP });
            }

            danhGia.MaTK = user.MaTK;
            danhGia.NgayDanhGia = DateTime.Now;
            db.DanhGia.Add(danhGia);
            db.SaveChanges();

            TempData["ReviewSuccess"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("ProductDetail", new { id = danhGia.MaSP });
        }
    }
}
