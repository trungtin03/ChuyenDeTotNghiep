using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Areas.Admin.Data;
using CDTN.Session;
using CDTN.Models;

namespace CDTN.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        storeDB db = new storeDB();
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginAccount loginAccount)
        {
            if (ModelState.IsValid)
            {
                TaiKhoanQuanTri tk = db.TaiKhoanQuanTri.Where(a => a.TenDangNhap.Equals(loginAccount.username)
                && a.MatKhau.Equals(loginAccount.password)).SingleOrDefault();
                if (tk != null)
                {
                    if(tk.TrangThai == false)
                    {
                        ModelState.AddModelError("ErrorLogin","Tài khoản đã bị vô hiệu hóa! Liên hệ quản trị viên!");
                    }
                    else
                    {
                        Session.Add(ConstaintUser.ADMIN_SESSION, tk);
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
            Session.Remove(ConstaintUser.ADMIN_SESSION);
            return RedirectToAction("Index");
        }
    }
}