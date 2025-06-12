using System;
using System.Linq;
using System.Web.Mvc;
using CDTN.Models;
using PagedList;

namespace CDTN.Areas.Admin.Controllers
{
    public class ThongBaoController : BaseController
    {
        storeDB db = new storeDB();

        [HttpGet]
        public ActionResult Index(string loai, int page = 1, int pageSize = 10)
        {
            var thongBaos = db.ThongBao.OrderByDescending(tb => tb.ThoiGian).AsQueryable();

            if (!string.IsNullOrEmpty(loai))
            {
                thongBaos = thongBaos.Where(tb => tb.Loai == loai);
            }

            ViewBag.Loai = loai;
            ViewBag.LoaiDanhSach = db.ThongBao.Select(t => t.Loai).Distinct().ToList();

            return View(thongBaos.ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult MarkAsRead(int id)
        {
            var tb = db.ThongBao.FirstOrDefault(x => x.ID == id);
            if (tb != null)
            {
                tb.DaXem = true;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = false });
        }
    }
}
