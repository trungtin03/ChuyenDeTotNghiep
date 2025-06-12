using System;
using System.Linq;
using System.Web.Mvc;
using CDTN.Models;

namespace CDTN.Areas.Admin.Controllers
{
    public class AdminSupportController : Controller
    {
        private storeDB db = new storeDB();

        // Trang chính: danh sách người dùng đã nhắn tin
        public ActionResult Index()
        {
            var userList = db.PhanHoiHoTro
                             .Where(m => m.MaTK != null)
                             .Select(m => m.TaiKhoanNguoiDung)
                             .Distinct()
                             .ToList();

            return View(userList);
        }

        // Lấy lịch sử tin nhắn của một người dùng
        public JsonResult GetUserMessages(int userId)
        {
            var rawMessages = db.PhanHoiHoTro
    .Where(p => p.MaTK == userId)
    .OrderBy(p => p.NgayGui)
    .ToList(); // lấy về trước

            // Sau đó xử lý bằng LINQ thường
            var messages = rawMessages.Select(p => new
            {
                noiDung = string.IsNullOrEmpty(p.PhanHoi) ? p.Message : p.PhanHoi,
                laAdmin = !string.IsNullOrEmpty(p.PhanHoi),
                ngayGui = p.NgayGui
            }).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);

        }


        // Gửi phản hồi từ admin
        [HttpPost]
        public JsonResult ReplyToUser(int userId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { success = false, message = "Nội dung không hợp lệ." });

            var reply = new PhanHoiHoTro
            {
                MaTK = userId,
                Subject = "Phản hồi từ admin",
                Message = message,
                NgayGui = DateTime.Now,
                TrangThai = 1,
                PhanHoi = message
            };

            db.PhanHoiHoTro.Add(reply);
            db.SaveChanges();

            return Json(new { success = true });
        }
        private TaiKhoanNguoiDung GetCurrentUser()
        {
            if (Session[CDTN.Session.ConstaintUser.USER_SESSION] != null)
            {
                return Session[CDTN.Session.ConstaintUser.USER_SESSION] as TaiKhoanNguoiDung;
            }
            return null;
        }

    }
}
