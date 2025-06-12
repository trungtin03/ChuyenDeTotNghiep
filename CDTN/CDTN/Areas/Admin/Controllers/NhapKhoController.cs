using System;
using System.Linq;
using System.Web.Mvc;
using CDTN.Models;
using System.Data.Entity;

namespace CDTN.Areas.Admin.Controllers
{
    public class AdminNhapKhoController : Controller
    {
        private storeDB db = new storeDB();

        // GET: Admin/AdminNhapKho
        public ActionResult Index()
        {
            var danhSach = db.NhapKho
                .Include(n => n.SanPhamChiTiet.SanPham)
                .Include(n => n.SanPhamChiTiet.MauSac)
                .Include(n => n.SanPhamChiTiet.KichCo)
                .OrderByDescending(n => n.NgayNhap)
                .Take(100)
                .ToList();

            return View(danhSach);
        }

        // GET: Admin/AdminNhapKho/Create
        public ActionResult Create()
        {
            ViewBag.IDCTSP = new SelectList(
                db.SanPhamChiTiet
                    .Include(s => s.SanPham)
                    .Include(s => s.MauSac)
                    .Include(s => s.KichCo)
                    .ToList()
                    .Select(s => new
                    {
                        s.IDCTSP,
                        TenChiTiet = s.SanPham.TenSP + " - " + s.MauSac.TenMau + " - " + s.KichCo.TenKichCo
                    }),
                "IDCTSP", "TenChiTiet");

            return View();
        }

        // POST: Admin/AdminNhapKho/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhapKho model)
        {
            if (ModelState.IsValid)
            {
                model.NgayNhap = DateTime.Now;
                db.NhapKho.Add(model);

                var ctsp = db.SanPhamChiTiet.Find(model.IDCTSP);
                if (ctsp != null)
                {
                    ctsp.SoLuong += model.SoLuongNhap;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Gửi lại ViewBag nếu có lỗi để dropdown không null
            ViewBag.IDCTSP = new SelectList(
                db.SanPhamChiTiet
                    .Include(s => s.SanPham)
                    .Include(s => s.MauSac)
                    .Include(s => s.KichCo)
                    .ToList()
                    .Select(s => new
                    {
                        s.IDCTSP,
                        TenChiTiet = s.SanPham.TenSP + " - " + s.MauSac.TenMau + " - " + s.KichCo.TenKichCo
                    }),
                "IDCTSP", "TenChiTiet", model.IDCTSP);

            return View(model);
        }
    }
}
