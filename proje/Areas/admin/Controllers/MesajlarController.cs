using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class MesajlarController : Controller
    {
        // GET: admin/Mesajlar
        public ActionResult Iletisim()
        {
            if (TempData["MesajSil"] != null)
            {
                ViewBag.MesajSil = TempData["MesajSil"].ToString();
            }
            if (TempData["MesajHata"] != null)
            {
                ViewBag.MesajHata = TempData["MesajHata"].ToString();
            }
            ViewBag.ActiveMenu = "Mesajlar";
            projeEntities1 db = new projeEntities1();
            var mesajlar = db.itetisim_mesajlari.ToList();

            return View(mesajlar.OrderByDescending(x=>x.durum).ThenByDescending(y=>y.tarih));
        }

        public ActionResult Iletisim_oku(int id)
        {

            projeEntities1 db = new projeEntities1();
            try {
                var mesaj = (from m in db.itetisim_mesajlari where m.id == id select m).First();
                mesaj.durum = "Okundu";
                db.SaveChanges();
                return View(mesaj);
            }
            catch
            {
                return RedirectToAction("Iletisim", "Mesajlar");
            }
          
        }

        public ActionResult Iletisim_sil(int id)
        {
            projeEntities1 db = new projeEntities1();
            try
            {
                var mesaj = (from m in db.itetisim_mesajlari where m.id == id select m).First();
                db.itetisim_mesajlari.Remove(mesaj);
                db.SaveChanges();
                TempData["MesajSil"] = "Mesaj Silindi.";
                return RedirectToAction("Iletisim", "Mesajlar");
            }
            catch
            {
                TempData["MesajHata"] = "Böyle bir mesaj bulunmamaktadır!";
                return RedirectToAction("Iletisim", "Mesajlar");
            }
        }
    }
}