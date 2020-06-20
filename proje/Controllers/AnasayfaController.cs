using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;
using proje.Models;

namespace proje.Controllers
{
    public class AnasayfaController : Controller
    {
        // GET: Anasayfa
        public ActionResult Index()
        {
            projeEntities1 db = new projeEntities1();

            var ayarlar = db.ayarlar.Where(x => x.id == 1).First();
            ViewBag.Title = ayarlar.title;
            ViewBag.Keywords = ayarlar.keywords;
            ViewBag.Description = ayarlar.description;

            if (TempData["catIDList"] != null)
            {
                ViewBag.catIDList = TempData["catIDList"];
            }

            if (TempData["autTimeList"] != null)
            {
                ViewBag.autTimeList = TempData["autTimeList"];
            }

            if (TempData["minMaxList"] != null)
            {
                ViewBag.minMaxList = TempData["minMaxList"];
            }


            return View(db.kategoriler.ToList());
        }

        [HttpPost]
        public ActionResult Filtreleme(FormCollection col)
        {
            var cat = col["kat[]"];
            var aucTime = col["aucTime[]"];
            var minF = col["minFiyat"];
            var maxF = col["maxFiyat"];

            List<int> catIDList = new List<int>();
            List<int> aucTimeList = new List<int>();
            List<double> minMaxList = new List<double>();

            if (cat != null)
            {
                var cat_p = col["kat[]"].Split(',').Select(x => int.Parse(x));
                foreach (var item in cat_p)
                {
                    catIDList.Add(item);
                }
                TempData["catIDList"] = catIDList;
            }

            if (aucTime != null)
            {
                var aucTime_p = col["aucTime[]"].Split(',').Select(x => int.Parse(x));
                foreach (var item in aucTime_p)
                {
                    aucTimeList.Add(item);
                }
                TempData["autTimeList"] = aucTimeList;
            }

            double minD, maxD;
            bool minCheck, maxCheck;
            minCheck = double.TryParse(minF.ToString(), out minD);
            maxCheck = double.TryParse(maxF.ToString(), out maxD);

            if (minF != "" || maxF != "")
            {
                if (minF != "" && minCheck)
                {
                    minMaxList.Add(Convert.ToDouble(minF));
                }
                else
                {
                    minMaxList.Add(0);
                }

                if (maxF != "" && maxCheck)
                {
                    minMaxList.Add(Convert.ToDouble(maxF));
                }
                else
                {
                    minMaxList.Add(0);
                }
                TempData["minMaxList"] = minMaxList;
            }


            return RedirectToAction("Index", "Anasayfa");
        }

        public ActionResult Teklif_zaman_kontrol(int urun_id)
        {
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == urun_id select u).First();
            DateTime localTime = DateTime.Now;
            DateTime startTime = Convert.ToDateTime(urun.baslangic_tarihi);
            DateTime finishTime = Convert.ToDateTime(urun.bitis_tarihi);

            if (localTime < startTime)
            {
                return RedirectToAction("Index", "Anasayfa");
            }
            else if (localTime >= startTime && localTime < finishTime)
            {
                if (urun.teklif_durumu == "Hayır")
                {
                    urun.teklif_durumu = "Evet";
                    db.SaveChanges();
                }

                return RedirectToAction("Acikarttirma", "Anasayfa", new { urun_id = urun.id });
            }
            else if (localTime >= finishTime)
            {
                if (urun.teklif_durumu == "Evet" && urun.yayin_durumu == "Evet")
                {
                    urun.teklif_durumu = "Hayır";
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Anasayfa");
            }

            return RedirectToAction("Index", "Anasayfa");
        }

        [GirisyapAuthorize]
        public ActionResult Acikarttirma(int urun_id)
        {
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == urun_id select u).First();
            DateTime localTime = DateTime.Now;
            DateTime startTime = Convert.ToDateTime(urun.baslangic_tarihi);
            DateTime finishTime = Convert.ToDateTime(urun.bitis_tarihi);


            if (localTime < startTime || localTime >= finishTime)
            {
                if (urun.teklif_durumu == "Evet")
                {
                    urun.teklif_durumu = "Hayır";
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Anasayfa");
            }

            var teklif_butonlari = (from t in db.urun_teklif_butonlari where t.urun_id == urun_id select t).OrderBy(x=>x.teklif_butonlari.miktar);
            ViewBag.urun = urun;

            return View(teklif_butonlari.ToList());
        }

        [GirisyapAuthorize]
        public JsonResult AuctionAdd(teklif_miktarlari auc)
        {
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == auc.urun_id select u).First();
            DateTime localTime = DateTime.Now;
            DateTime startTime = Convert.ToDateTime(urun.baslangic_tarihi);
            DateTime finishTime = Convert.ToDateTime(urun.bitis_tarihi);


            if (localTime < startTime || localTime >= finishTime)
            {
                if (urun.teklif_durumu == "Evet")
                {
                    urun.teklif_durumu = "Hayır";
                    db.SaveChanges();
                }
                return Json(false);
            }


            AuctionDB aucDB = new AuctionDB();
            return Json(aucDB.AuctionAdd(auc), JsonRequestBehavior.AllowGet);
        }

        [GirisyapAuthorize]
        public JsonResult BakiyeGetir(int uye_id, int urun_id)
        {
            AuctionDB aucDB = new AuctionDB();
            return Json(aucDB.BakiyeGetir(uye_id, urun_id), JsonRequestBehavior.AllowGet);
        }

        [GirisyapAuthorize]
        public JsonResult MaxTeklif(int urun_id)
        {
            AuctionDB aucDB = new AuctionDB();
            return Json(aucDB.MaxTeklif(urun_id), JsonRequestBehavior.AllowGet);
        }

        [GirisyapAuthorize]
        public JsonResult Teklifler(int urun_id)
        {
            AuctionDB aucDB = new AuctionDB();
            return Json(new { data1 = aucDB.Teklifler(urun_id), data2 = aucDB.MaxTeklif(urun_id), data3 = aucDB.ToplamTeklifler(urun_id) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(int p_start, int p_finish,List<string> catIDList, List<string> autTimeList, double min, double max)
        {
            AuctionDB aucDB = new AuctionDB();
            return Json(new { products = aucDB.GetProducts(p_start,p_finish, catIDList, autTimeList,min,max), localtime = aucDB.GetLocalTime() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult BizeYazin()
        {
            if (TempData["iletisim_mesaj"] != null)
            {
                ViewBag.iletisim_mesaj = TempData["iletisim_mesaj"].ToString();
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BizeYazin(FormCollection col)
        {
            string localIP = Request.UserHostAddress.ToString();

            var adsoy = col["adsoy"];
            var email = col["email"];
            var telefon = col["telefon"];
            var mesaj = col["mesaj"];

            projeEntities1 db = new projeEntities1();
            var iletisim = new itetisim_mesajlari();
            iletisim.adsoy = adsoy;
            iletisim.email = email;
            iletisim.tel = telefon;
            iletisim.mesaj = mesaj;
            iletisim.ip = localIP;
            iletisim.durum = "Yeni";
            iletisim.tarih = DateTime.Now;

            db.itetisim_mesajlari.Add(iletisim);
            db.SaveChanges();

            TempData["iletisim_mesaj"] = "Mesajınız iletildi.";

            return RedirectToAction("BizeYazin","Anasayfa");
        }

        public ActionResult NasilCalisir()
        {
            projeEntities1 db = new projeEntities1();
            var ayarlar = db.ayarlar.Where(x => x.id == 1).First();
            return View(ayarlar);
        }

        public ActionResult Iletisim()
        {
            projeEntities1 db = new projeEntities1();
            var ayarlar = db.ayarlar.Where(x => x.id == 1).First();
            return View(ayarlar);
        }

        public ActionResult Hakkinda()
        {
            projeEntities1 db = new projeEntities1();
            var ayarlar = db.ayarlar.Where(x => x.id == 1).First();
            return View(ayarlar);
        }

        public ActionResult Sosyal(int s)
        {
            projeEntities1 db = new projeEntities1();
            var ayarlar = db.ayarlar.Where(x => x.id == 1).First();
            string url = string.Empty;

            try {
                if (s == 1) //Facebook
                {
                    url = ayarlar.facebook;
                }
                if (s == 2) //Twitter
                {
                    url = ayarlar.twitter;
                }
                if (s == 3) //Instagram
                {
                    url = ayarlar.instagram;
                }

                return Redirect(url);
            }
            catch {
                return RedirectToAction("Index", "Anasayfa");
            }
            

        }
    }

}