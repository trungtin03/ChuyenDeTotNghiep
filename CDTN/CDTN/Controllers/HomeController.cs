using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Areas.Admin.Data;
using CDTN.Session;
using CDTN.Models;
using BCrypt.Net;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace CDTN.Controllers
{
    public class HomeController : Controller
    {
        storeDB db = new storeDB();
        // GET: Home
        public ActionResult Index()
        {
            // Sản phẩm mới nhất theo từng danh mục
            var sanPhamMoi = db.SanPham
                .GroupBy(p => p.MaDM)
                .Select(g => g.OrderByDescending(p => p.NgayTao).FirstOrDefault())
                .ToList();

            // Sản phẩm giá tốt nhất theo từng danh mục
            var giaTot = db.SanPham
                .GroupBy(p => p.MaDM)
                .Select(g => g.OrderBy(p => p.Gia).FirstOrDefault())
                .ToList();

            // Thời trang Nam – lấy tối đa 10 sản phẩm chứa từ "nam"
            var thoiTrangNam = db.SanPham
                .Where(p => p.TenSP.ToLower().Contains("nam"))
                .Take(10)
                .ToList();

            // Thời trang Nữ – lấy tối đa 10 sản phẩm chứa từ "nữ"
            var thoiTrangNu = db.SanPham
                .Where(p => p.TenSP.ToLower().Contains("nữ"))
                .Take(10)
                .ToList();

            ViewBag.SanPhamMoi = sanPhamMoi;
            ViewBag.GiaTot = giaTot;
            ViewBag.ThoiTrangNam = thoiTrangNam;
            ViewBag.ThoiTrangNu = thoiTrangNu;

            return View();
        }



        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            IEnumerable<DanhMuc> danhmucs = db.DanhMuc.Select(p => p);
            return PartialView(danhmucs);
        }

        [ChildActionOnly]
        public ActionResult DropdownCategories()
        {
            IEnumerable<DanhMuc> danhmucs = db.DanhMuc.Select(p => p);
            return PartialView(danhmucs);
        }

        [ChildActionOnly]
        public ActionResult SelectOptionSize()
        {
            IEnumerable<KichCo> kichCos = db.KichCo.Select(p => p);
            return PartialView(kichCos);
        }

        [ChildActionOnly]
        public ActionResult CartCount()
        {
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            list = (List<ChiTietHoaDon>)Session[CDTN.Session.ConstainCart.CART];
            return PartialView(list);
        }


        [HttpGet]
        public ActionResult Login()
        {
            TaiKhoanNguoiDung session = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (session != null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginAccount loginAccount)
        {
            if (ModelState.IsValid)
            {
                TaiKhoanNguoiDung tk = db.TaiKhoanNguoiDung
                    .Where(a => a.TenDangNhap.Equals(loginAccount.username))
                    .FirstOrDefault();

                if (tk != null && BCrypt.Net.BCrypt.Verify(loginAccount.password, tk.MatKhau))
                {
                    if (tk.TrangThai == false)
                    {
                        ModelState.AddModelError("ErrorLogin", "Tài khoản của bạn đã bị vô hiệu hóa!");
                    }
                    else
                    {
                        Session.Add(ConstaintUser.USER_SESSION, tk);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorLogin", "Tài khoản hoặc mật khẩu không đúng!");
                }

                if (tk != null)
                {
                    if(tk.TrangThai == false)
                    {
                        ModelState.AddModelError("ErrorLogin","Tài khoản của bạn đã bị vô hiệu hóa !");
                    }
                    else
                    {
                        Session.Add(ConstaintUser.USER_SESSION, tk);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorLogin", "Tài khoản hoặc mật khẩu không đúng!");
                }
            }
            return View(loginAccount);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove(ConstaintUser.USER_SESSION);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            TaiKhoanNguoiDung session = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (session != null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(TaiKhoanNguoiDung tk)
        {
            TaiKhoanNguoiDung check = db.TaiKhoanNguoiDung.Where
                (a => a.TenDangNhap.Equals(tk.TenDangNhap)).FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("ErrorSignUp", "Tên đăng nhập đã tồn tại");
            }
            else
            {
                try
                {
                    tk.TrangThai = false;
                    tk.MatKhau = BCrypt.Net.BCrypt.HashPassword(tk.MatKhau);
                    db.TaiKhoanNguoiDung.Add(tk);
                    db.SaveChanges();

                    // Không cho login liền
                    ViewBag.Message = "Đăng ký thành công. Vui lòng chờ quản trị viên kích hoạt tài khoản.";
                    return View();

                }
                catch (Exception)
                {
                    ModelState.AddModelError("ErrorSignUp", "Đăng ký không thành công. Thử lại sau !");
                }
            }

            return View(tk);
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.TaiKhoanNguoiDung.FirstOrDefault(x => x.Email == email && x.TrangThai == true);
            if (user == null)
            {
                ModelState.AddModelError("ErrorForgot", "Không tìm thấy tài khoản với email này hoặc tài khoản chưa được kích hoạt.");
                return View();
            }

            string token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);
            db.SaveChanges();

            try
            {
                SendResetPasswordEmail(user.Email, user.HoTen, token);
                ViewBag.Message = "Email đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra hộp thư.";
            }
            catch (Exception)
            {
                ModelState.AddModelError("ErrorForgot", "Không thể gửi email. Vui lòng thử lại sau.");
            }

            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var user = db.TaiKhoanNguoiDung.FirstOrDefault(x => x.ResetPasswordToken == token && x.ResetTokenExpiry > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return View("ResetPasswordError");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, string newPassword)
        {
            var user = db.TaiKhoanNguoiDung.FirstOrDefault(x => x.ResetPasswordToken == token && x.ResetTokenExpiry > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return View("ResetPasswordError");
            }

            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;
            db.SaveChanges();

            ViewBag.Message = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập lại.";
            return RedirectToAction("Login");
        }
        private void SendResetPasswordEmail(string toEmail, string fullName, string token)
        {
            string emailUser = ConfigurationManager.AppSettings["EmailUser"];
            string emailPass = ConfigurationManager.AppSettings["EmailPassword"];

            var fromEmail = new MailAddress(emailUser, "True Store");
            var toEmailAddress = new MailAddress(toEmail);

            string resetLink = Url.Action("ResetPassword", "Home", new { token = token }, Request.Url.Scheme);
            string subject = "Đặt lại mật khẩu";
            string body = $@"
        <h2>Khôi phục mật khẩu</h2>
        <p>Chào {fullName},</p>
        <p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào nút bên dưới để đặt lại:</p>
        <p><a href='{resetLink}' style='background-color:#28a745;color:white;padding:10px 15px;text-decoration:none;border-radius:5px;'>Đặt lại mật khẩu</a></p>
        <p>Liên kết có hiệu lực trong 30 phút.</p>
        <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
        <br/>
        <p>Trân trọng,<br/>True Mart</p>";

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