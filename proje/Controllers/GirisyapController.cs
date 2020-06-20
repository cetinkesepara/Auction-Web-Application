using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Controllers
{
    public class GirisyapController : Controller
    {
        // GET: GirisYap
        public ActionResult Index()
        {
            if(Session["uye_email"] != null)
            {
                return RedirectToAction("Index", "Anasayfa");
            }

            if (TempData["email_hata"] != null)
            {
                ViewBag.email_hata = TempData["email_hata"].ToString();
            }

            if (TempData["kayit_onay"] != null)
            {
                ViewBag.kayit_onay = TempData["kayit_onay"].ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            
                 string email = collection["email"];
                 string sifre = collection["sifre"];

                 projeEntities1 db = new projeEntities1();
                 var uye = from u in db.uyeler where u.email == email && u.sifre == sifre select u;

                 if (uye.Count() == 1)
                 {
                     var oturum = uye.First();
                     Session["uye_adsoy"] = oturum.adsoy;
                     Session["uye_id"] = oturum.id;
                     Session["uye_resim"] = oturum.resim;
                     Session["uye_email"] = oturum.email;
                     Session["uye_bakiye"] = oturum.bakiye;



                     return RedirectToAction("Index", "Anasayfa");
                 }

                 ViewBag.LoginError = "Email adresiniz veya şifreniz yanlış!";
                 
            return View();


        }

        public ActionResult Cikisyap()
        {
            Session.Remove("uye_adsoy");
            Session.Remove("uye_id");
            Session.Remove("uye_resim");
            Session.Remove("uye_email");
            Session.Remove("uye_bakiye");
            return RedirectToAction("Index", "Girisyap");
        }

        [HttpPost]
        public ActionResult KayitOl(FormCollection col)
        {
            var uyeAdi = col["uyeAdi"];
            var soyAd = col["soyAd"];
            var adsoy = uyeAdi + " " + soyAd;
            var email = col["email"];
            var email_tekrar = col["email_tekrar"];
            var sifre = col["sifre"];
            var sifre_tekrar = col["sifre_tekrar"];
            var telefon = col["telefon"];
            var sehir = col["sehir"];
            var adres = col["adres"];

            if(email == email_tekrar && sifre == sifre_tekrar)
            {
                projeEntities1 db = new projeEntities1();
                var email_var = db.uyeler.ToList().Where(x=>x.email == email);
                if(email_var.Count() == 0)
                {
                    var uye = new uyeler();
                    uye.adsoy = adsoy;
                    uye.email = email;
                    uye.sifre = sifre;
                    uye.telefon = telefon;
                    uye.sehir = sehir;
                    uye.adres = adres;
                    uye.resim = "default_profile.jpg";
                    uye.yetki = "Üye";
                    uye.durum = "Aktif";
                    uye.tarih = DateTime.Now;
                    uye.bakiye = 0;

                    db.uyeler.Add(uye);
                    db.SaveChanges();

                    TempData["kayit_onay"] = "Kaydınız oluşturuldu şimdi giriş yapabilirsiniz.";

                }
                else
                {
                    TempData["email_hata"] = email + " email hesabı ile zaten kayıt olunmuş.";
                 
                }
            }

            return RedirectToAction("Index", "Girisyap");
        }
    }
}