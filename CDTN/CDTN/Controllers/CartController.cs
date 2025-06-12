using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using System.Data.Entity;

namespace CDTN.Controllers
{
    public class CartController : Controller
    {
        storeDB db = new storeDB();

        [HttpGet]
        public ActionResult Orders()
        {
            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            if (Session[CDTN.Session.ConstainCart.CART] != null)
            {
                List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in ses)
                {
                    var spct = db.SanPhamChiTiet
                        .Include(ct => ct.SanPham)
                        .Include(ct => ct.KichCo)
                        .FirstOrDefault(ct => ct.IDCTSP == item.IDCTSP);

                    if (spct != null)
                    {
                        var mau = db.SanPhamMau
                            .Where(sm => sm.MaSP == spct.MaSP)
                            .Select(sm => new { sm.MauSac.TenMau, sm.MauSac.MaMauHex })
                            .FirstOrDefault();

                        if (mau != null)
                        {
                            spct.SanPham.ChatLieu = mau.TenMau;
                            spct.SanPham.HinhAnh = mau.MaMauHex;
                        }

                        list.Add(spct);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].ChiTietHoaDon.Add(ses[i]);
                }
            }
            return View(list);
        }

        [HttpPost]
        public JsonResult AddToCart(ChiTietHoaDon chiTiet)
        {
            var sanPhamChiTiet = db.SanPhamChiTiet.FirstOrDefault(x => x.IDCTSP == chiTiet.IDCTSP);

            if (sanPhamChiTiet == null || chiTiet.SoLuongMua > sanPhamChiTiet.SoLuong)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            bool isExists = false;
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();

            if (Session[CDTN.Session.ConstainCart.CART] != null)
            {
                list = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in list)
                {
                    if (item.IDCTSP == chiTiet.IDCTSP)
                    {
                        item.SoLuongMua += chiTiet.SoLuongMua;
                        isExists = true;
                    }
                }
                if (!isExists)
                {
                    list.Add(chiTiet);
                }
            }
            else
            {
                list = new List<ChiTietHoaDon> { chiTiet };
            }

            list.RemoveAll(x => x.SoLuongMua <= 0);

            foreach (ChiTietHoaDon item in list)
            {
                var sanpham = db.SanPhamChiTiet.Include(ct => ct.SanPham).FirstOrDefault(ct => ct.IDCTSP == item.IDCTSP);
                if (sanpham != null)
                {
                    item.GiaMua = sanpham.SanPham.Gia;
                }
            }

            Session[CDTN.Session.ConstainCart.CART] = list;
            return Json(new { status = true, cart = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFromCart(int idctsp)
        {
            List<ChiTietHoaDon> list = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];
            list.RemoveAll(x => x.IDCTSP == idctsp);
            Session[CDTN.Session.ConstainCart.CART] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (tk == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            if (Session[CDTN.Session.ConstainCart.CART] != null)
            {
                List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in ses)
                {
                    var spct = db.SanPhamChiTiet
                        .Include(ct => ct.SanPham)
                        .Include(ct => ct.KichCo)
                        .FirstOrDefault(ct => ct.IDCTSP == item.IDCTSP);

                    if (spct != null)
                    {
                        var mau = db.SanPhamMau
                            .Where(sm => sm.MaSP == spct.MaSP)
                            .Select(sm => new { sm.MauSac.TenMau, sm.MauSac.MaMauHex })
                            .FirstOrDefault();

                        if (mau != null)
                        {
                            spct.SanPham.ChatLieu = mau.TenMau;
                            spct.SanPham.HinhAnh = mau.MaMauHex;
                        }

                        list.Add(spct);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].ChiTietHoaDon.Add(ses[i]);
                }
            }

            ViewBag.TaiKhoan = tk;
            return View(list);
        }

        [HttpGet]
        public ActionResult BuyNow(int id, int quantity)
        {
            var item = db.SanPhamChiTiet.Include(x => x.SanPham).FirstOrDefault(x => x.IDCTSP == id);
            if (item == null || item.SoLuong < quantity)
            {
                return RedirectToAction("ProductDetail", "Product", new { id = item?.MaSP });
            }

            var list = new List<ChiTietHoaDon>
            {
                new ChiTietHoaDon
                {
                    IDCTSP = id,
                    SoLuongMua = quantity,
                    GiaMua = item.SanPham.Gia
                }
            };

            Session[CDTN.Session.ConstainCart.CART] = list;
            return RedirectToAction("CheckOut", "Cart");
        }
    }
}
