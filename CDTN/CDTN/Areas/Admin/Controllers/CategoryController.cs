using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;
using PagedList;
using CDTN.Helpers; 


namespace CDTN.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        storeDB db = new storeDB();

        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            ViewBag.searchString = searchString;
            var danhmucs = db.DanhMuc.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                danhmucs = danhmucs.Where(dm => dm.TenDanhMuc.Contains(searchString));
            }
            return View(danhmucs.OrderBy(dm => dm.MaDM).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult Index(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                var result = db.DanhMuc
                    .Where(x => x.MaDM == id)
                    .Select(x => new
                    {
                        x.MaDM,
                        x.TenDanhMuc,
                        x.NgayTao,
                        x.NguoiTao,
                        x.NgaySua,
                        x.NguoiSua
                    })
                    .FirstOrDefault();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Create(DanhMuc dm)
        {
            TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];

            if (!PhanQuyenHelper.CoQuyen(tk.ID, "Thêm danh mục"))
            {
                return Json(new { status = false, message = "Bạn không có quyền thêm danh mục" });
            }

            try
            {
                dm.NgayTao = DateTime.Now;
                dm.NguoiTao = tk.HoTen;
                dm.NgaySua = DateTime.Now;
                dm.NguoiSua = tk.HoTen;
                db.DanhMuc.Add(dm);
                db.SaveChanges();

                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã thêm danh mục: {dm.TenDanhMuc}",
                    ThoiGian = DateTime.Now,
                    Loai = "danh mục",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true, message = "Thêm thành công" });
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Tên danh mục đã tồn tại" });
            }
        }


        [HttpPost]
        public JsonResult Update(DanhMuc dm)
        {
            TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];

            if (!PhanQuyenHelper.CoQuyen(tk.ID, "Sửa danh mục"))
            {
                return Json(new { status = false, message = "Bạn không có quyền sửa danh mục" });
            }

            try
            {
                DanhMuc update = db.DanhMuc.FirstOrDefault(a => a.MaDM == dm.MaDM);
                update.TenDanhMuc = dm.TenDanhMuc;
                update.NgaySua = DateTime.Now;
                update.NguoiSua = tk.HoTen;
                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();

                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã sửa danh mục: {update.TenDanhMuc}",
                    ThoiGian = DateTime.Now,
                    Loai = "danh mục",
                    DaXem = false
                });
                db.SaveChanges();

                return Json(new { status = true, message = "Sửa thông tin thành công" });
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Tên danh mục đã tồn tại" });
            }
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[CDTN.Session.ConstaintUser.ADMIN_SESSION];

            if (!PhanQuyenHelper.CoQuyen(tk.ID, "Xóa danh mục"))
            {
                return Json(new { status = false, message = "Bạn không có quyền xóa danh mục" });
            }

            try
            {
                DanhMuc dm = db.DanhMuc.FirstOrDefault(a => a.MaDM == id);
                db.DanhMuc.Remove(dm);
                db.SaveChanges();

                db.ThongBao.Add(new ThongBao
                {
                    NoiDung = $"Admin đã xóa danh mục: {dm.TenDanhMuc}",
                    ThoiGian = DateTime.Now,
                    Loai = "danh mục",
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
