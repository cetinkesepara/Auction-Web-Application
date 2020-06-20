using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class KargolarController : Controller
    {
        public ActionResult FirmaKargolari()
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "FirmaKargolari";
            projeEntities1 db = new projeEntities1();
            var kargolar = from k in db.kargolar where k.tur == "Firma-Üye" select k;

            return View(kargolar.ToList().OrderByDescending(x => x.tarih));
        }

        public ActionResult Firma_kargo_islem(int kargo_id)
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "FirmaKargolari";
            projeEntities1 db = new projeEntities1();
            var kargo = from k in db.kargolar where k.tur == "Firma-Üye" && k.id == kargo_id select k;

            return View(kargo.First());
        }

        [HttpPost]
        public ActionResult Firma_kargo_islem(FormCollection col, int kargo_id)
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "FirmaKargolari";
            projeEntities1 db = new projeEntities1();
            try
            {
                int kargo_islem = Convert.ToInt32(col["kargo_islem"]);

                var kargo = (from k in db.kargolar where k.tur == "Firma-Üye" && k.id == kargo_id select k).First();
                var teklif = (from t in db.teklifler where t.urun_id == kargo.urun_id select t).First();

                if (kargo_islem == 1)
                {
                    kargo.durum = "Kargo Yola Çıktı";
                    db.SaveChanges(); 

                }
                if(kargo_islem == 2)
                {
                    kargo.durum = "Kargo Teslim Edildi";
                    db.SaveChanges();
                }

                if(kargo_islem == 3)
                {
                    db.teklifler.Remove(teklif);
                    db.kargolar.Remove(kargo);
                    db.SaveChanges();
                }

            }
            catch
            {
                return RedirectToAction("FirmaKargolari", "Kargolar");
            }


            return RedirectToAction("FirmaKargolari", "Kargolar");
        }

        public ActionResult SatUyeKargolari()
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "SatUyeKargolari";
            projeEntities1 db = new projeEntities1();
            var kargolar = from k in db.kargolar where k.tur == "Satıcı-Üye" select k;

            return View(kargolar.ToList().OrderByDescending(x=>x.tarih));
        }

        public ActionResult SatUye_kargo_islem(int kargo_id)
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "SatUyeKargolari";
            projeEntities1 db = new projeEntities1();
            var kargo = from k in db.kargolar where k.tur == "Satıcı-Üye" && k.id == kargo_id select k;

            return View(kargo.First());
        }

        [HttpPost]
        public ActionResult SatUye_kargo_islem(FormCollection col,int kargo_id)
        {
            ViewBag.ActiveMenu = "Kargolar";
            ViewBag.ActiveSubMenu = "SatUyeKargolari";
            projeEntities1 db = new projeEntities1();
            try {
                int kargo_islem = Convert.ToInt32(col["kargo_islem"]);
                double komisyon = Convert.ToDouble(col["komisyon"]);

                var kargo = (from k in db.kargolar where k.tur == "Satıcı-Üye" && k.id == kargo_id select k).First();
                var teklif = (from t in db.teklifler where t.urun_id == kargo.urun_id select t).First();
                var satici = (from u in db.uyeler where u.id == kargo.urunler.uye_id select u).First();

                if(kargo_islem == 1 && komisyon < teklif.teklif)
                {
                    int satici_id = kargo.urunler.uye_id;
                    var urun_adi = kargo.urunler.adi;
                    double kazanc = teklif.teklif - komisyon;
                    satici.bakiye += kazanc;
                    db.SaveChanges();

                    db.teklifler.Remove(teklif);
                    db.kargolar.Remove(kargo);
                    db.SaveChanges();

                    var mesaj = new mesajlasmalar();
                    mesaj.uye_id = satici_id;
                    mesaj.tarih = DateTime.Now;
                    mesaj.mesaj = "<p> '" + urun_adi + "' isimli ürününüzün açık arttırmasından '"+ komisyon +"' TL komisyon bedeli düşülerek '"+ kazanc +"' TL kazandınız. Bu miktar bakiyenize eklenmiştir.</p>";
                    mesaj.durum = "Yeni";
                    mesaj.tur = "Alınan";
                    mesaj.mesaj_turu = "Satış Sonucu";

                    db.mesajlasmalar.Add(mesaj);
                    db.SaveChanges();

                }
            }
            catch
            {
                return RedirectToAction("SatUyeKargolari", "Kargolar");
            }


            return RedirectToAction("SatUyeKargolari", "Kargolar");
        }

    }
}