using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDTN.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Title = "Liên hệ";
            ViewBag.Breadcrumb = "Liên Hệ";
            ViewBag.ActiveMenu = "contact";
            return View();
        }
        public ActionResult BuyGuide()
        {
            ViewBag.Title = "Hướng Dẫn Mua Hàng";
            return View();
        }
        public ActionResult SizeGuide()
        {
            ViewBag.Title = "Hướng dẫn chọn size";
            return View();
        }
        public ActionResult Policy()
        {
            ViewBag.Title = "Điều khoản & Chính sách";
            return View();
        }
        public ActionResult Faq()
        {
            ViewBag.Title = "Câu hỏi thường gặp";
            return View();
        }
    }
}