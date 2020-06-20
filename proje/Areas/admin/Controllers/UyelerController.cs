using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class UyelerController : Controller
    {
        // GET: admin/Uyeler
        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "Uyeler";
            if(TempData["UyeEklemeSonucu"] != null)
            {
                ViewBag.UyeEklemeSonucu = TempData["UyeEklemeSonucu"].ToString();
            }
            if(TempData["UyeDuzenlemeSonucu"] != null)
            {
                ViewBag.UyeDuzenlemeSonucu = TempData["UyeDuzenlemeSonucu"].ToString();
            }
            if(TempData["UyeSilmeSonucu"] != null)
            {
                ViewBag.UyeSilmeSonucu = TempData["UyeSilmeSonucu"].ToString();
            }

            projeEntities1 db = new projeEntities1();

            return View(db.uyeler.ToList().OrderByDescending(x => x.id));
        }

        // GET: admin/Uyeler/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.ActiveMenu = "Uyeler";
            projeEntities1 db = new projeEntities1();
            var uye = from u in db.uyeler where u.id == id select u; 

            return View(uye.First());
        }

        // GET: admin/Uyeler/Create
        public ActionResult Create()
        {
            ViewBag.ActiveMenu = "Uyeler";
            return View();
        }

        // POST: admin/Uyeler/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var uyelerModel = new uyeler();
                uyelerModel.adsoy = collection["adsoy"];
                uyelerModel.email = collection["email"];
                uyelerModel.sifre = collection["sifre"];
                uyelerModel.telefon = collection["telefon"];
                uyelerModel.sehir = collection["sehir"];
                uyelerModel.adres = collection["adres"];
                uyelerModel.yetki = collection["yetki"];
                uyelerModel.durum = collection["durum"];
                uyelerModel.resim = "default_profile.jpg";

                DateTime tarih = DateTime.Now;
                uyelerModel.tarih = Convert.ToDateTime(tarih);

                projeEntities1 db = new projeEntities1();
                db.uyeler.Add(uyelerModel);
                db.SaveChanges();

                TempData["UyeEklemeSonucu"] = collection["adsoy"] + " adlı üyeyi eklediniz.";

                return RedirectToAction("Index","Uyeler");
            }
            catch
            {
                return View();
            }
        }

        // GET: admin/Uyeler/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.ActiveMenu = "Uyeler";
            projeEntities1 db = new projeEntities1();
            var uye = from u in db.uyeler where u.id == id select u; 

            return View(uye.First());
        }

        // POST: admin/Uyeler/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                var uye = (from u in db.uyeler where u.id == id select u).First();

                uye.adsoy = collection["adsoy"];
                uye.email = collection["email"];
                uye.sifre = collection["sifre"];
                uye.telefon = collection["telefon"];
                uye.sehir = collection["sehir"];
                uye.adres = collection["adres"];
                uye.yetki = collection["yetki"];
                uye.durum = collection["durum"];

                DateTime tarih = DateTime.Now;
                uye.tarih = Convert.ToDateTime(tarih);

                db.SaveChanges();

                TempData["UyeDuzenlemeSonucu"] = collection["adsoy"] + " adlı üyeyi güncellediniz.";

                return RedirectToAction("Index", "Uyeler");
            }
            catch
            {
                return View();
            }
        }

        // GET: admin/Uyeler/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                string resim_yol;
                var uye = (from u in db.uyeler where u.id == id select u).First();

                db.mesajlasmalar.RemoveRange(db.mesajlasmalar.Where(x=>x.uye_id == uye.id));
                db.SaveChanges();

                db.teklifler.RemoveRange(db.teklifler.Where(x => x.uye_id == uye.id));
                db.SaveChanges();

                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(x => x.uye_id == uye.id));
                db.SaveChanges();

                db.kargolar.RemoveRange(db.kargolar.Where(x => x.uye_id == uye.id));
                db.SaveChanges();

                var urunler = (from u in db.urunler where u.uye_id == id select u).ToList();

                foreach (var item in urunler)
                {
                    db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(x => x.urun_id == item.id));

                    db.teklifler.RemoveRange(db.teklifler.Where(x => x.urun_id == item.id));

                    db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(x => x.urun_id == item.id));

                    db.kargolar.RemoveRange(db.kargolar.Where(x => x.urun_id == item.id));

                    resim_yol = "~/Assets/uploads/products_img/" + item.resim;
                    if (System.IO.File.Exists(Server.MapPath(resim_yol)))
                    {
                        System.IO.File.Delete(Server.MapPath(resim_yol));
                    }

                    db.urunler.RemoveRange(db.urunler.Where(x => x.id == item.id));
                    db.SaveChanges();
                }

             

                string resim_yol2 = "~/Assets/uploads/profile_img/" + uye.resim;
                if (System.IO.File.Exists(Server.MapPath(resim_yol2)) && uye.resim != "default_profile.jpg")
                {
                    System.IO.File.Delete(Server.MapPath(resim_yol2));
                }
                db.uyeler.Remove(uye);
                db.SaveChanges();

                TempData["UyeSilmeSonucu"] = uye.adsoy + " adlı üyeyi sildiniz.";

                return RedirectToAction("Index", "Uyeler");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Yuklemeler()
        {
            ViewBag.ActiveMenu = "Yuklemeler";
            if (TempData["by_islem"] != null)
            {
                ViewBag.by_islem = TempData["by_islem"].ToString();
            }

            if (TempData["by_sil"] != null)
            {
                ViewBag.by_sil = TempData["by_sil"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            
            return View(db.bakiye_yukle.ToList().OrderByDescending(x=>x.tarih));
        }

        public ActionResult Yuklemeler_islem(int b_id)
        {
            projeEntities1 db = new projeEntities1();
            var by = (from b in db.bakiye_yukle where b.id == b_id select b).First();
            int uye_id = by.uye_id;
            ViewBag.uyeadi = (from u in db.uyeler where u.id == uye_id  select u.adsoy).First();

            return View(by);
        }

        [HttpPost]
        public ActionResult Yuklemeler_islem(FormCollection col,int b_id)
        {
            projeEntities1 db = new projeEntities1();
            var islem = col["islem"];
            var by = db.bakiye_yukle.Where(x => x.id == b_id).First();
            int uye_id = by.uye_id;

            if(islem == "Onaylandı" && by.durum == "Onay Bekleniyor")
            {
                var uye = (from u in db.uyeler where u.id == uye_id select u).First();
                uye.bakiye += by.miktar;
                by.durum = islem;
                db.SaveChanges();

                TempData["by_islem"] = "Yükleme işlemi onaylandı."; 
            }
            if(islem == "Reddedildi" && by.durum == "Onay Bekleniyor")
            {
                by.durum = islem;
                db.SaveChanges();
                TempData["by_islem"] = "Yükleme işlemi reddedildi.";
            }
            return RedirectToAction("Yuklemeler","Uyeler");
        }

        public ActionResult Yuklemeler_sil(int id)
        {
            projeEntities1 db = new projeEntities1();
            db.bakiye_yukle.RemoveRange(db.bakiye_yukle.Where(x => x.id == id));
            db.SaveChanges();

            TempData["by_sil"] = "Bakiye bilgisi silindi.";

            return RedirectToAction("Yuklemeler", "Uyeler");
        }

    }
}
