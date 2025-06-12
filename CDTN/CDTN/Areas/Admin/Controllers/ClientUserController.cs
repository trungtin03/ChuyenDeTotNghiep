using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CDTN.Models;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace CDTN.Areas.Admin.Controllers
{
    public class ClientUserController : BaseController
    {
        storeDB db = new storeDB();

        [HttpGet]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            ViewBag.searchString = searchString;
            ViewBag.ActiveTab = "all";
            var taikhoans = db.TaiKhoanNguoiDung.Where(tk => tk.TrangThai == true);
            if (!String.IsNullOrEmpty(searchString))
            {
                taikhoans = taikhoans.Where(tk => tk.TenDangNhap.Contains(searchString));
            }
            return View(taikhoans.OrderBy(tk => tk.MaTK).ToPagedList(page, pageSize));
        }

        [HttpGet]
        public ActionResult Pending(string searchString, int page = 1, int pageSize = 5)
        {
            ViewBag.searchString = searchString;
            ViewBag.ActiveTab = "pending";
            var pendingAccounts = db.TaiKhoanNguoiDung.Where(tk => tk.TrangThai == false);
            if (!String.IsNullOrEmpty(searchString))
            {
                pendingAccounts = pendingAccounts.Where(tk => tk.TenDangNhap.Contains(searchString));
            }
            return View("Index", pendingAccounts.OrderBy(tk => tk.MaTK).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult Update(int Matk)
        {
            try
            {
                TaiKhoanNguoiDung update = db.TaiKhoanNguoiDung.FirstOrDefault(a => a.MaTK == Matk);
                if (update == null)
                {
                    return Json(new { status = false, message = "Không tìm thấy tài khoản." });
                }

                update.TrangThai = !update.TrangThai;
                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();

                if (update.TrangThai)
                {
                    // Gửi email
                    SendActivationEmail(update.Email, update.TenDangNhap, update.HoTen);

                    // ✅ Ghi thông báo kích hoạt
                    db.ThongBao.Add(new ThongBao
                    {
                        NoiDung = $"Admin đã kích hoạt tài khoản người dùng: {update.TenDangNhap}",
                        ThoiGian = DateTime.Now,
                        Loai = "người dùng",
                        DaXem = false
                    });
                    db.SaveChanges();
                }

                return Json(new { status = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                TaiKhoanNguoiDung tk = db.TaiKhoanNguoiDung.FirstOrDefault(a => a.MaTK == id);
                if (tk != null)
                {
                    db.TaiKhoanNguoiDung.Remove(tk);
                    db.SaveChanges();

                    // ✅ Ghi thông báo xóa tài khoản
                    db.ThongBao.Add(new ThongBao
                    {
                        NoiDung = $"Admin đã xóa tài khoản người dùng: {tk.TenDangNhap}",
                        ThoiGian = DateTime.Now,
                        Loai = "người dùng",
                        DaXem = false
                    });
                    db.SaveChanges();
                }

                return Json(new { status = true });
            }
            catch (Exception)
            {
                return Json(new { status = false });
            }
        }

        private void SendActivationEmail(string toEmail, string username, string fullName)
        {
            string emailUser = ConfigurationManager.AppSettings["EmailUser"];
            string emailPass = ConfigurationManager.AppSettings["EmailPassword"];

            var fromEmail = new MailAddress(emailUser, "True Store");
            var toEmailAddress = new MailAddress(toEmail);

            string subject = "Tài khoản của bạn đã được kích hoạt";
            string body = $@"
                <h2>Kích hoạt tài khoản thành công!</h2>
                <p>Chào {fullName} ({username}),</p>
                <p>Tài khoản của bạn đã được <b>kích hoạt</b></p>
                <br/>
                <p>Trân trọng,<br/>Your Shop</p>";

            using (var message = new MailMessage(fromEmail, toEmailAddress))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailUser, emailPass);
                    smtp.Send(message);
                }
            }
        }
    }
}
