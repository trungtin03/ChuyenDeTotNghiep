using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CDTN.Models;

namespace CDTN.Areas.Admin.Controllers
{
    public class QuyenController : Controller
    {
        private storeDB db = new storeDB();

        // GET: Admin/Quyen
        public ActionResult Index()
        {
            var quyens = db.Quyen.Include(q => q.ChucNang).ToList();
            return View(quyens);
        }

        public ActionResult Create()
        {
            ViewBag.MaCN = new SelectList(db.ChucNang, "MaCN", "TenChucNang");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenQuyen,MaCN")] Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Quyen.Add(quyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaCN = new SelectList(db.ChucNang, "MaCN", "TenChucNang", quyen.MaCN);
            return View(quyen);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Quyen quyen = db.Quyen.Find(id);
            if (quyen == null)
                return HttpNotFound();

            ViewBag.MaCN = new SelectList(db.ChucNang, "MaCN", "TenChucNang", quyen.MaCN);
            return View(quyen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaQuyen,TenQuyen,MaCN")] Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaCN = new SelectList(db.ChucNang, "MaCN", "TenChucNang", quyen.MaCN);
            return View(quyen);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Quyen quyen = db.Quyen.Find(id);
                if (quyen == null)
                    return Json(new { success = false, message = "Không tìm thấy quyền." });

                db.Quyen.Remove(quyen);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ✅ TRẢ JSON cho popup phân quyền
        public JsonResult GanQuyen(int id)
        {
            // Lấy danh sách quyền, bỏ trùng theo MaQuyen (nếu có)
            var danhSachQuyen = db.Quyen
                .Include(q => q.ChucNang)
                .GroupBy(q => q.MaQuyen)
                .Select(g => g.FirstOrDefault())
                .ToList();

            var quyenDaGan = db.PhanQuyen
                .Where(p => p.IDTaiKhoan == id)
                .Select(p => p.MaQuyen)
                .ToList();

            var result = new
            {
                danhSachQuyen = danhSachQuyen.Select(q => new
                {
                    q.MaQuyen,
                    q.TenQuyen,
                    ChucNang = new
                    {
                        q.ChucNang.TenChucNang
                    }
                }),
                quyenDaGan = quyenDaGan
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LuuPhanQuyen(int IDTaiKhoan, List<int> quyenIds)
        {
            if (quyenIds == null) quyenIds = new List<int>(); // ✅ Fix null khi không chọn gì

            var quyenCu = db.PhanQuyen.Where(p => p.IDTaiKhoan == IDTaiKhoan).ToList();
            db.PhanQuyen.RemoveRange(quyenCu);
            db.SaveChanges();

            foreach (var maQuyen in quyenIds)
            {
                db.PhanQuyen.Add(new PhanQuyen
                {
                    IDTaiKhoan = IDTaiKhoan,
                    MaQuyen = maQuyen
                });
            }
            db.SaveChanges();

            return Json(new { success = true });
        }

    }
}
