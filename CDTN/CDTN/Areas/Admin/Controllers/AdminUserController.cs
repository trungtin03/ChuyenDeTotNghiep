using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using System.Data.Entity;
using PagedList;

namespace CDTN.Areas.Admin.Controllers
{
    public class AdminUserController : BaseController
    {
        storeDB db = new storeDB();

        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            TaiKhoanQuanTri ses = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
            if (ses.LoaiTaiKhoan == false)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.searchString = searchString;
            var taikhoans = db.TaiKhoanQuanTri.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                taikhoans = taikhoans.Where(tk => tk.TenDangNhap.Contains(searchString));
            }
            return View(taikhoans.OrderBy(tk => tk.ID).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult Create(TaiKhoanQuanTri tk)
        {
            try
            {
                TaiKhoanQuanTri check = db.TaiKhoanQuanTri.FirstOrDefault(a => a.TenDangNhap == tk.TenDangNhap);
                if (check != null)
                {
                    return Json(new { status = false, message = "Tên đăng nhập đã tồn tại!" });
                }

                tk.TrangThai = true;
                db.TaiKhoanQuanTri.Add(tk);
                db.SaveChanges();

                // ✅ Ghi log
                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã thêm tài khoản quản trị: {tk.TenDangNhap}",
                    ThoiGian = DateTime.Now,
                    Loai = "quản trị",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true, message = "Thêm thành công!" });
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Thêm không thành công!" });
            }
        }

        [HttpPost]
        public JsonResult GetById(int id)
        {
            try
            {
                var tk = db.TaiKhoanQuanTri.FirstOrDefault(a => a.ID == id);
                if (tk == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        ID = tk.ID,
                        TenDangNhap = tk.TenDangNhap,
                        HoTen = tk.HoTen,
                        LoaiTaiKhoan = tk.LoaiTaiKhoan,
                        TrangThai = tk.TrangThai
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(TaiKhoanQuanTri tk)
        {
            TaiKhoanQuanTri login = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
            try
            {
                TaiKhoanQuanTri update = db.TaiKhoanQuanTri.FirstOrDefault(a => a.ID == tk.ID);
                if (update.TenDangNhap == login.TenDangNhap)
                {
                    return Json(new { status = false, message = "Bạn không thể sửa tài khoản này !" });
                }

                update.LoaiTaiKhoan = tk.LoaiTaiKhoan;
                update.TrangThai = tk.TrangThai;
                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();

                // ✅ Ghi log
                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã cập nhật tài khoản quản trị: {update.TenDangNhap}",
                    ThoiGian = DateTime.Now,
                    Loai = "quản trị",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true, message = "Sửa thông tin thành công" });
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Có lỗi gì đó. Thử lại sau !" });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            TaiKhoanQuanTri login = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];
            try
            {
                TaiKhoanQuanTri tk = db.TaiKhoanQuanTri.FirstOrDefault(a => a.ID == id);
                if (tk.TenDangNhap == login.TenDangNhap)
                {
                    return Json(new { status = false, message = "Bạn không thể xóa tài khoản này !" });
                }

                db.TaiKhoanQuanTri.Remove(tk);
                db.SaveChanges();

                // ✅ Ghi log
                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã xóa tài khoản quản trị: {tk.TenDangNhap}",
                    ThoiGian = DateTime.Now,
                    Loai = "quản trị",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true });
            }
            catch (Exception)
            {
                return Json(new { status = false });
            }
        }
    }
}
