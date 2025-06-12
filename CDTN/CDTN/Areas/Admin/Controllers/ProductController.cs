using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CDTN.Models;
using CDTN.Helpers;
using PagedList;

namespace CDTN.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        storeDB db = new storeDB();

        [HttpGet]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            ViewBag.Category = db.DanhMuc.ToList();
            ViewBag.Size = db.KichCo.ToList();
            ViewBag.MauSac = db.MauSac.ToList();
            ViewBag.ColorList = db.MauSac.ToList();

            ViewBag.searchString = searchString;
            var sanphams = db.SanPham
                .Include("DanhMuc")
                .Include("SanPhamMaus.MauSac")
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(sp => sp.TenSP.Contains(searchString));
            }

            return View(sanphams.OrderBy(sp => sp.MaSP).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult GetSanPhamById(int id) 
        {
            var sp = db.SanPham
                .Include(x => x.SanPhamChiTiet.Select(ct => ct.KichCo))
                .Include(x => x.SanPhamChiTiet.Select(ct => ct.MauSac))
                .FirstOrDefault(x => x.MaSP == id);

            if (sp == null) return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                sp.MaSP,
                sp.MaDM,
                sp.TenSP,
                sp.Gia,
                sp.ChatLieu,
                sp.MoTa,
                sp.HuongDan,
                sp.HinhAnh,
                ListMauSize = sp.SanPhamChiTiet.Select(x => new
                {
                    MaMau = x.MaMau,
                    MaKichCo = x.MaKichCo,
                    SoLuong = x.SoLuong
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Create(string sanpham, string chiTietMauSize, HttpPostedFileBase hinhanh)
        {
            try
            {
                JavaScriptSerializer convert = new JavaScriptSerializer();
                var sp = convert.Deserialize<SanPham>(sanpham);
                var listChiTiet = convert.Deserialize<List<ChiTietMauSize>>(chiTietMauSize);

                TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];

                // Xử lý ảnh
                if (hinhanh != null && hinhanh.ContentLength > 0)
                {
                    string fileName = new Random().Next() + System.IO.Path.GetFileName(hinhanh.FileName);
                    string uploadPath = Server.MapPath("~/UserImage/images/" + fileName);
                    hinhanh.SaveAs(uploadPath);
                    sp.HinhAnh = "/UserImage/images/" + fileName;
                }
                else
                {
                    sp.HinhAnh = "/UserImage/images/noimage.jpg";
                }

                sp.NgayTao = DateTime.Now;
                sp.NguoiTao = tk.HoTen;
                sp.NgaySua = DateTime.Now;
                sp.NguoiSua = tk.HoTen;

                db.SanPham.Add(sp);
                db.SaveChanges();

                int masp = sp.MaSP;

                foreach (var item in listChiTiet)
                {
                    var ctsp = new SanPhamChiTiet
                    {
                        MaSP = masp,
                        MaKichCo = item.MaKichCo,
                        MaMau = item.MaMau,
                        SoLuong = 0
                    };

                    db.SanPhamChiTiet.Add(ctsp);
                    db.SaveChanges();

                    db.NhapKho.Add(new NhapKho
                    {
                        IDCTSP = ctsp.IDCTSP,
                        SoLuongNhap = item.SoLuong,
                        NgayNhap = DateTime.Now
                    });

                    ctsp.SoLuong += item.SoLuong;
                }

                db.SaveChanges(); // Lưu toàn bộ nhập kho và cộng tồn


                return Json(new { status = true, message = "Thêm sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Thêm sản phẩm thất bại! " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(string sanpham, string chiTietMauSize, HttpPostedFileBase hinhanh)
        {
            try
            {
                JavaScriptSerializer convert = new JavaScriptSerializer();
                var sp = convert.Deserialize<SanPham>(sanpham);
                var listChiTiet = convert.Deserialize<List<ChiTietMauSize>>(chiTietMauSize);

                TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
                var update = db.SanPham.FirstOrDefault(x => x.MaSP == sp.MaSP);

                if (update == null)
                    return Json(new { status = false, message = "Không tìm thấy sản phẩm!" });

                // Xử lý ảnh nếu có upload mới
                if (hinhanh != null && hinhanh.ContentLength > 0)
                {
                    string fileName = new Random().Next() + System.IO.Path.GetFileName(hinhanh.FileName);
                    string uploadPath = Server.MapPath("~/UserImage/images/" + fileName);
                    hinhanh.SaveAs(uploadPath);
                    update.HinhAnh = "/UserImage/images/" + fileName;
                }

                update.TenSP = sp.TenSP;
                update.MaDM = sp.MaDM;
                update.Gia = sp.Gia;
                update.ChatLieu = sp.ChatLieu;
                update.MoTa = sp.MoTa;
                update.HuongDan = sp.HuongDan;
                update.NgaySua = DateTime.Now;
                update.NguoiSua = tk.HoTen;

                db.Entry(update).State = EntityState.Modified;

                // Xóa hết chi tiết cũ
                var chiTietCu = db.SanPhamChiTiet.Where(x => x.MaSP == sp.MaSP).ToList();
                db.SanPhamChiTiet.RemoveRange(chiTietCu);
                db.SaveChanges();
                foreach (var item in listChiTiet)
                {
                    var ctsp = new SanPhamChiTiet
                    {
                        MaSP = sp.MaSP,
                        MaKichCo = item.MaKichCo,
                        MaMau = item.MaMau,
                        SoLuong = item.SoLuong
                    };

                    db.SanPhamChiTiet.Add(ctsp);
                    db.SaveChanges(); // để lấy IDCTSP

                    // Ghi nhận lịch sử nhập mới
                    db.NhapKho.Add(new NhapKho
                    {
                        IDCTSP = ctsp.IDCTSP,
                        SoLuongNhap = item.SoLuong,
                        NgayNhap = DateTime.Now
                    });
                }

                db.SaveChanges();

                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = "Admin đã cập nhật sản phẩm: " + update.TenSP,
                    ThoiGian = DateTime.Now,
                    Loai = "sản phẩm",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
            if (!PhanQuyenHelper.CoQuyen(tk.ID, "Xóa sản phẩm"))
                return Json(new { status = false, message = "Bạn không có quyền xóa sản phẩm!" });

            try
            {
                var sp = db.SanPham.Find(id);
                if (sp == null) return Json(new { status = false });

                db.SanPham.Remove(sp);
                db.SaveChanges();

                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = "Admin đã xóa sản phẩm: " + sp.TenSP,
                    ThoiGian = DateTime.Now,
                    Loai = "sản phẩm",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { status = false });
            }
        }
    }
}
