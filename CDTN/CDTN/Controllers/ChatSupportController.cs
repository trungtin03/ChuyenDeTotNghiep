using System;
using System.Linq;
using System.Web.Mvc;
using CDTN.Models;
using CDTN.Session; // phải using đúng để dùng ConstaintUser

namespace CDTN.Controllers
{
    public class ChatSupportController : Controller
    {
        private storeDB db = new storeDB(); // dùng storeDB

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMessage(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Json(new { success = false, message = "Nội dung không được để trống." });
                }

                var currentUser = GetCurrentUser();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập." });
                }

                var newMessage = new PhanHoiHoTro
                {
                    MaTK = currentUser.MaTK,
                    Subject = "Tin nhắn hỗ trợ",
                    Message = message,
                    NgayGui = DateTime.Now,
                    TrangThai = 0,
                    PhanHoi = null
                };

                db.PhanHoiHoTro.Add(newMessage);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Tin nhắn đã được gửi!",
                    data = new
                    {
                        id = newMessage.ID,
                        subject = newMessage.Subject,
                        message = newMessage.Message,
                        ngayGui = newMessage.NgayGui.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        private TaiKhoanNguoiDung GetCurrentUser()
        {
            if (Session[CDTN.Session.ConstaintUser.USER_SESSION] != null)
            {
                return Session[CDTN.Session.ConstaintUser.USER_SESSION] as TaiKhoanNguoiDung;
            }
            return null;
        }
        [HttpGet]
        public JsonResult GetUserMessages()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return Json(new { success = false, message = "Bạn chưa đăng nhập." }, JsonRequestBehavior.AllowGet);

            var messages = db.PhanHoiHoTro
                .Where(p => p.MaTK == currentUser.MaTK)
                .OrderBy(p => p.NgayGui)
                .Select(p => new
                {
                    noiDung = p.PhanHoi != null ? p.PhanHoi : p.Message,
                    laAdmin = p.PhanHoi != null,
                    thoiGian = p.NgayGui
                })
                .ToList();

            return Json(new { success = true, data = messages }, JsonRequestBehavior.AllowGet);
        }

    }
}
