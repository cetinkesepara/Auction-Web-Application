using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class KategorilerController : Controller
    {
        // GET: admin/Kategoriler
        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "Kategoriler";

            if (TempData["KategoriEkleSonuc"] != null)
            {
                ViewBag.KategoriEkleSonuc = TempData["KategoriEkleSonuc"].ToString();
            }
            if (TempData["KategoriEkleHata"] != null)
            {
                ViewBag.KategoriEkleHata = TempData["KategoriEkleHata"].ToString();
            }
            if (TempData["KategoriSil"] != null)
            {
                ViewBag.KategoriSil = TempData["KategoriSil"].ToString();
            }
            if (TempData["KategoriSilHata"] != null)
            {
                ViewBag.KategoriSilHata = TempData["KategoriSilHata"].ToString();
            }


            projeEntities1 db = new projeEntities1();
            return View(db.kategoriler.ToList());
        }

        [HttpPost]
        public ActionResult KategoriEkle(FormCollection col)
        {
            projeEntities1 db = new projeEntities1();
            var kategori = col["kategori"];
            var kat = (from k in db.kategoriler where k.adi == kategori select k).ToList();
            if(kat.Count() == 0)
            {
                var kt = new kategoriler();
                kt.adi = kategori;
                kt.description = col["description"];
                kt.keywords = col["keywords"];
                kt.tarih = DateTime.Now;
                kt.durum = "Aktif";
                kt.parent_id = 0;
                db.kategoriler.Add(kt);
                db.SaveChanges();

                TempData["KategoriEkleSonuc"] = kategori + " kategorisi eklendi.";
            }
            else
            {
                TempData["KategoriEkleHata"] = kategori + " adında zaten bir kategori bulunmaktadır.";
            }

            return RedirectToAction("Index", "Kategoriler");
        }

        public ActionResult KategoriSil(int id)
        {
            projeEntities1 db = new projeEntities1();

            var kat_var = (from u in db.urunler where u.kategori_id == id select u).ToList();
            if(kat_var.Count() == 0)
            {
                db.kategoriler.RemoveRange(db.kategoriler.Where(x => x.id == id));
                db.SaveChanges();

                TempData["KategoriSil"] = "Kategori silindi";
            }
            else
            {
                TempData["KategoriSilHata"] = "Silmeye çalıştığınız kategori, başka ürünlerde kullanılıyor. Silmek için önce bu ürünlerin kategorilerini değiştirmeli veya ürünü silmelisiniz.";
            }

            return RedirectToAction("Index", "Kategoriler");
        }
    }
}