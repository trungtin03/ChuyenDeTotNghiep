﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDTN.Models;

namespace CDTN.Controllers
{
    public class AccountUserController : Controller
    {
        storeDB  db = new storeDB();
        // GET: AccountUser
        [HttpGet]
        public ActionResult ChangePassWord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassWord(string oldpassword, string password)
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if(tk.MatKhau != oldpassword)
            {
                ModelState.AddModelError("ErrorUpdate","Mật khẩu cũ không đúng");
            }
            else
            {
                TaiKhoanNguoiDung edit = db.TaiKhoanNguoiDung.Where(a => a.MaTK.Equals(tk.MaTK)).FirstOrDefault();
                edit.MatKhau = password;
                db.SaveChanges();
                Session[CDTN.Session.ConstaintUser.USER_SESSION] = edit;
                ModelState.AddModelError("ErrorUpdate", "Đổi mật khẩu thành công!");
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserInfor(int id)
        {
            TaiKhoanNguoiDung session = (TaiKhoanNguoiDung)Session[CDTN.Session.ConstaintUser.USER_SESSION];
            if (session == null)
            {
                return RedirectToAction("PageNotFound","Error");
            }
            else
            {
                TaiKhoanNguoiDung tk = db.TaiKhoanNguoiDung.Where(a => a.MaTK.Equals(id)).FirstOrDefault();
                return View(tk);
            }
        }

        [HttpPost]
        public ActionResult UserInfor([Bind(Include = "MaTK,Hoten,Diachi,,Email,Sodienthoai,Ngaysinh,Gioitinh")] TaiKhoanNguoiDung tk)
        {
            TaiKhoanNguoiDung edit = db.TaiKhoanNguoiDung.Where(a => a.MaTK.Equals(tk.MaTK)).FirstOrDefault();
            try
            {
                edit.HoTen = tk.HoTen;
                edit.DiaChi = tk.DiaChi;
                edit.Email = tk.Email;
                edit.SoDienThoai = tk.SoDienThoai;
                edit.NgaySinh = tk.NgaySinh;
                edit.GioiTinh = tk.GioiTinh;
                db.SaveChanges();
                Session[CDTN.Session.ConstaintUser.USER_SESSION] = edit;
            }
            catch(Exception)
            {
                ModelState.AddModelError("ErrorUpdate","Cập nhật thông tin không thành công ! Thử lại sau !");
            }
            return View(edit);
        }
    }
}