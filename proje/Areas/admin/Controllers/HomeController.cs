using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class HomeController : Controller
    {
        // GET: admin/Home
        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "Anasayfa";


            projeEntities1 db = new projeEntities1();
            int uyeId = Convert.ToInt32(Session["id"]);
            var profil = from p in db.uyeler where p.id == uyeId select p;

            return View(profil.First());
        }
            
        public ActionResult Profil()
        {
            projeEntities1 db = new projeEntities1();
            int uyeId = Convert.ToInt32(Session["id"]);
            var profil = from p in db.uyeler where p.id == uyeId select p;

            return View(profil.First());
        }

        public ActionResult Profil_Resmi_Degis()
        {
            if (TempData["ProfilResimYuklemeSonucu"] != null)
            {
                ViewBag.ProfilResimYuklemeSonucu = TempData["ProfilResimYuklemeSonucu"].ToString();
            }
            if (TempData["ResimTuruHata"] != null)
            {
                ViewBag.ResimTuruHata = TempData["ResimTuruHata"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            int uyeId = Convert.ToInt32(Session["id"]);
            var profil = from p in db.uyeler where p.id == uyeId select p;

            return View(profil.First());
        }

        [HttpPost]
        public ActionResult Profil_Resmi_Degis(HttpPostedFileBase imageFile)
        {

            try
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    if (imageFile.ContentType == "image/jpeg" || imageFile.ContentType == "image/png")
                    {
                        projeEntities1 db = new projeEntities1();
                        var resim_adlari = (from r in db.uyeler select r.resim).ToList();

                        int index = 0;
                        int img_count = 1;
                        int count = resim_adlari.Count();
                        string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string imageNameTemp = imageName;
                        do
                        {
                            string resimAdi = System.IO.Path.GetFileNameWithoutExtension(resim_adlari[index]);
                            if (imageName == resimAdi)
                            {
                                imageName = imageNameTemp + img_count.ToString();
                                img_count++;
                                index = -1;
                            }

                            index++;

                        } while (index < count);

                        string path = System.IO.Path.Combine(Server.MapPath("~/Assets/uploads/profile_img"), imageName + ".jpg");
                        imageFile.SaveAs(path);

                    
                        string resim_adi = imageName + ".jpg";
                        int sess_id = Convert.ToInt32(Session["id"]);
                        var uye = (from u in db.uyeler where u.id == sess_id select u).First();
                        string resim_yol = "~/Assets/uploads/profile_img/" + uye.resim;
                        if (System.IO.File.Exists(Server.MapPath(resim_yol)) && uye.resim != "default_profile.jpg")
                        {
                            System.IO.File.Delete(Server.MapPath(resim_yol));
                        }

                        uye.resim = resim_adi;

                        db.SaveChanges();

                        TempData["ProfilResimYuklemeSonucu"] = resim_adi + " adlı resim yüklendi.";

                        return RedirectToAction("Profil_Resmi_Degis", "Home");
                    }
                    else
                    {
                        TempData["ResimTuruHata"] = "Sadece jpeg ve png türündeki resimleri yükleyebilirsiniz";
                        return RedirectToAction("Profil_Resmi_Degis", "Home");
                    }

                }

                return RedirectToAction("Profil_Resmi_Degis", "Home");
            }
            catch
            {
                return RedirectToAction("Profil_Resmi_Degis", "Home");
            }
            
        }

        public ActionResult Kisisel_Bilgi_Degis()
        {
            if (TempData["ProfilDüzenlemeSonucu"] != null)
            {
                ViewBag.ProfilDüzenlemeSonucu = TempData["ProfilDüzenlemeSonucu"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            int uyeId = Convert.ToInt32(Session["id"]);
            var profil = from p in db.uyeler where p.id == uyeId select p;

            return View(profil.First());
        }

        [HttpPost]
        public ActionResult Kisisel_Bilgi_Degis(FormCollection collection)
        {
            try { 
                projeEntities1 db = new projeEntities1();
                int uyeId = Convert.ToInt32(Session["id"]);
                var profil = (from p in db.uyeler where p.id == uyeId select p).First();

                profil.adsoy = collection["adsoy"];
                profil.sifre = collection["sifre"];
                profil.telefon = collection["tel"];
                profil.sehir = collection["sehir"];
                profil.adres = collection["adres"];

                db.SaveChanges();

                TempData["ProfilDüzenlemeSonucu"] = "Profil bilgileriniz güncellendi.";

                return RedirectToAction("Kisisel_Bilgi_Degis", "Home");
            }
            catch
            {
                return RedirectToAction("Kisisel_Bilgi_Degis", "Home");
            }
        }

        public ActionResult Ayarlar() {
            if(TempData["ayarlarGuncelle"] != null)
            {
                ViewBag.ayarlarGuncelle = TempData["ayarlarGuncelle"];
            }

            ViewBag.ActiveMenu = "Ayarlar";
            projeEntities1 db = new projeEntities1();
            var ayarlar = from a in db.ayarlar where a.id == 1 select a;
            return View(ayarlar.First());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Ayarlar(FormCollection col)
        {
            var site_adi = col["site_adi"];
            var site_baslik = col["site_baslik"];
            var keywords = col["keywords"];
            var description = col["description"];
            var adres = col["adres"];
            var sehir = col["sehir"];
            var tel = col["tel"];
            var email = col["email"];
            var hakkimizda = col["hakkimizda"];
            var iletisim = col["iletisim"];
            var nasil_calisir = col["nasil_calisir"];
            var facebook = col["facebook"];
            var twitter = col["twitter"];
            var instagram = col["instagram"];

            projeEntities1 db = new projeEntities1();
            var ayarlar = (from a in db.ayarlar where a.id == 1 select a).First();
            ayarlar.site_adi = site_adi;
            ayarlar.title = site_baslik;
            ayarlar.keywords = keywords;
            ayarlar.description = description;
            ayarlar.adres = adres;
            ayarlar.sehir = sehir;
            ayarlar.tel = tel;
            ayarlar.email = email;
            ayarlar.hakkimizda = hakkimizda;
            ayarlar.iletisim = iletisim;
            ayarlar.nasil_calisir = nasil_calisir;
            ayarlar.facebook = facebook;
            ayarlar.twitter = twitter;
            ayarlar.instagram = instagram;
            db.SaveChanges();

            TempData["ayarlarGuncelle"] = "Ayarlar güncellendi";

            return RedirectToAction("Ayarlar","Home");
        }
    }
}