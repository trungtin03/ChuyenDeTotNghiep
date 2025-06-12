using CDTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CDTN.Helpers;

namespace CDTN.Areas.Admin.Controllers
{
    public class DiscountCodeController : Controller
    {
        private storeDB db = new storeDB();

        // GET: DiscountCode
        public ActionResult Index()
        {
            var list = db.DiscountCodes.Include("Product").ToList();
            return View(list);
        }

        // GET: DiscountCode/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.SanPham.ToList(), "MaSP", "TenSP");
            return View();
        }

        // POST: DiscountCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                db.DiscountCodes.Add(discountCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.SanPham.ToList(), "MaSP", "TenSP", discountCode.ProductId);
            return View(discountCode);
        }

        // GET: DiscountCode/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var discount = db.DiscountCodes.Find(id);
            if (discount == null)
                return HttpNotFound();

            ViewBag.ProductId = new SelectList(db.SanPham.ToList(), "MaSP", "TenSP", discount.ProductId);
            return View(discount);
        }

        // POST: DiscountCode/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discountCode).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.SanPham.ToList(), "MaSP", "TenSP", discountCode.ProductId);
            return View(discountCode);
        }

        // GET: DiscountCode/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var discount = db.DiscountCodes.Find(id);
            if (discount == null)
                return HttpNotFound();

            return View(discount);
        }

        // POST: DiscountCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var discount = db.DiscountCodes.Find(id);
            db.DiscountCodes.Remove(discount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}