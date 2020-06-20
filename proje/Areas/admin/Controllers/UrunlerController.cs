using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class UrunlerController : Controller
    {
        public ActionResult FirmaUrunleri()
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "FirmaUrunleri";

            if (TempData["UrunEklemeSonucu"] != null)
            {
                ViewBag.UrunEklemeSonucu = TempData["UrunEklemeSonucu"].ToString();
            }
            if (TempData["UrunDuzenlemeSonucu"] != null)
            {
                ViewBag.UrunDuzenlemeSonucu = TempData["UrunDuzenlemeSonucu"].ToString();
            }
            if (TempData["UrunSilmeSonucu"] != null)
            {
                ViewBag.UrunSilmeSonucu = TempData["UrunSilmeSonucu"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            var firma_urunler = from u in db.urunler where u.uyeler.yetki == "Admin" || u.uyeler.yetki == "Yönetici" select u;

            return View(firma_urunler.ToList().OrderBy(x => x.durum).ThenBy(t => t.tarih));
        }

        public ActionResult FirmaUrunEkle()
        {
            if (TempData["ResimTuruHata"] != null)
            {
                ViewBag.ResimTuruHata = TempData["ResimTuruHata"].ToString();
            }

            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "FirmaUrunleri";
            projeEntities1 db = new projeEntities1();

            var kategoriler = from u in db.kategoriler select u;
            ViewBag.Kategoriler = kategoriler.ToList();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FirmaUrunEkle(FormCollection collection, HttpPostedFileBase imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    if (imageFile.ContentType == "image/jpeg" || imageFile.ContentType == "image/png")
                    {
                        myfuctions func = new myfuctions();
                        string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName);
                        imageName = func.sameNameImageChange(imageName);

                        string path = System.IO.Path.Combine(Server.MapPath("~/Assets/uploads/products_img"), imageName + ".jpg");
                        imageFile.SaveAs(path);

                        var urunModel = new urunler();
                        urunModel.resim = imageName + ".jpg";
                        urunModel.adi = collection["adi"];
                        urunModel.stok = 1;
                        urunModel.taban_fiyati = Convert.ToDecimal(collection["taban_fiyati"]);
                        urunModel.tavan_fiyati = Convert.ToDecimal(collection["tavan_fiyati"]);
                        urunModel.kategori_id = Convert.ToInt32(collection["kategori"]);
                        urunModel.durum = collection["durum"];
                        urunModel.description = collection["description"];
                        urunModel.keywords = collection["keywords"];
                        urunModel.aciklama = collection["aciklama"];
                        urunModel.tarih = DateTime.Now;
                        urunModel.yayin_durumu = "Hayır";
                        urunModel.teklif_durumu = "Hayır";
                        urunModel.onay_durumu = "Evet";
                        urunModel.uye_id = Convert.ToInt32(Session["id"]);

                        projeEntities1 db = new projeEntities1();
                        db.urunler.Add(urunModel);
                        db.SaveChanges();


                        TempData["UrunEklemeSonucu"] = collection["adi"] + " adlı ürünü eklediniz.";

                        return RedirectToAction("FirmaUrunleri", "Urunler");

                    }
                    else
                    {
                        TempData["ResimTuruHata"] = "Sadece jpeg ve png türündeki resimleri yükleyebilirsiniz";
                        return RedirectToAction("FirmaUrunEkle", "Urunler");
                    }

                }

                return RedirectToAction("FirmaUrunEkle", "Urunler");
            }
            catch
            {
                return RedirectToAction("FirmaUrunEkle", "Urunler");
            }
        }

        public ActionResult FirmaUrunYayinla(int id)
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "FirmaUrunleri";

            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var zamanlar = from u in db.zamanlar select u;
            var teklif_butonlari = (from u in db.teklif_butonlari select u).OrderBy(x=>x.miktar);
            var urun = from u in db.urunler where u.id == id && u.yayin_durumu == "Hayır" && u.onay_durumu == "Evet" select u;

            if (urun.Count() <= 0)
            {
                return RedirectToAction("FirmaUrunleri", "Urunler");
            }

            ViewBag.urun = urun.First();
            ViewBag.teklif_butonlari = teklif_butonlari.ToList();

            return View(zamanlar.ToList().OrderByDescending(x => x.zaman_dilimi).ThenBy(x => Convert.ToInt32(x.zaman)));
        }

        [HttpPost]
        public ActionResult FirmaUrunYayinla(FormCollection collection,int id)
        {
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == id select u).First();

            DateTime baslangic_tarihi = DateTime.Now, bitis_tarihi = DateTime.Now;

            if (Convert.ToInt32(collection["baslangic_tarihi"]) == -1)
            {
                baslangic_tarihi = Convert.ToDateTime(collection["bas_tarihi"]);
            }
            else
            {
                if (Convert.ToInt32(collection["baslangic_tarihi"]) != 0)
                {
                    int z_id = Convert.ToInt32(collection["baslangic_tarihi"]);
                    projeEntities1 db2 = new projeEntities1();
                    var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                    if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                    {
                        baslangic_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                    }
                    if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                    {
                        baslangic_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                    }
                }
            }

            urun.baslangic_tarihi = Convert.ToDateTime(baslangic_tarihi);


            if (Convert.ToInt32(collection["bitis_tarihi"]) == -1)
            {
                bitis_tarihi = Convert.ToDateTime(collection["bit_tarihi"]);
            }
            else
            {
                if (Convert.ToInt32(collection["bitis_tarihi"]) != 0)
                {
                    int z_id = Convert.ToInt32(collection["bitis_tarihi"]);
                    projeEntities1 db2 = new projeEntities1();
                    var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                    if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                    {
                        bitis_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                    }
                    if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                    {
                        bitis_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                    }
                }
            }

            urun.bitis_tarihi = Convert.ToDateTime(bitis_tarihi);

            urun.yayin_durumu = "Evet";

            var button = (from b in db.teklif_butonlari select b).ToList();
            var urun_button = new urun_teklif_butonlari();

            int colCount = collection.Count;

            foreach (var item in button)
            {
                for (int c = 0; c < colCount - 2; c++)
                {
                    if (item.id == Convert.ToInt32(collection[c]))
                    {
                        urun_button.urun_id = id;
                        urun_button.teklif_butonu_id = item.id;
                        db.urun_teklif_butonlari.Add(urun_button);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("FirmaUrunleri", "Urunler");
        }

        public ActionResult FirmaUrunlerDetay(int id)
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "FirmaUrunleri";

            projeEntities1 db = new projeEntities1();
            var urun = from u in db.urunler where u.id == id && (u.uyeler.yetki == "Admin" || u.uyeler.yetki == "Yönetici") select u;

            return View(urun.First());
        }

        public ActionResult FirmaUrunlerDuzenle(int id)
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "FirmaUrunleri";
            projeEntities1 db = new projeEntities1();

            var urun = from u in db.urunler where u.id == id && (u.uyeler.yetki == "Admin" || u.uyeler.yetki == "Yönetici") select u;
            ViewBag.Urun = urun.First();

            var kategoriler = from u in db.kategoriler select u;
            ViewBag.Kategoriler = kategoriler.ToList();

            var zamanlar = from u in db.zamanlar select u;
            return View(zamanlar.ToList().OrderByDescending(x => x.zaman_dilimi).ThenBy(x => Convert.ToInt32(x.zaman)));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FirmaUrunlerDuzenle(int id, FormCollection collection)
        {
            try
            {
                int check_bitNull = 0, check_basNull = 0;
                projeEntities1 db = new projeEntities1();
                var urun = (from u in db.urunler where u.id == id select u).First();

                urun.adi = collection["adi"];
                urun.stok = 1;
                urun.taban_fiyati = Convert.ToDecimal(collection["taban_fiyati"]);
                urun.tavan_fiyati = Convert.ToDecimal(collection["tavan_fiyati"]);

                DateTime baslangic_tarihi = DateTime.Now, bitis_tarihi = DateTime.Now;

                if (Convert.ToInt32(collection["baslangic_tarihi"]) == -1)
                {
                    if (collection["bas_tarihi"] != "")
                        baslangic_tarihi = Convert.ToDateTime(collection["bas_tarihi"]);
                    else
                        check_basNull = 1;
                }
                else
                {
                    if (Convert.ToInt32(collection["baslangic_tarihi"]) != 0)
                    {
                        int z_id = Convert.ToInt32(collection["baslangic_tarihi"]);
                        projeEntities1 db2 = new projeEntities1();
                        var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                        if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                        {
                            baslangic_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                        }
                        if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                        {
                            baslangic_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                        }
                    }
                }

                if(check_basNull != 1)
                    urun.baslangic_tarihi = Convert.ToDateTime(baslangic_tarihi);


                if (Convert.ToInt32(collection["bitis_tarihi"]) == -1)
                {
                    if (collection["bit_tarihi"] != "")
                        bitis_tarihi = Convert.ToDateTime(collection["bit_tarihi"]);
                    else
                        check_bitNull = 1;
                }
                else
                {
                    if (Convert.ToInt32(collection["bitis_tarihi"]) != 0)
                    {
                        int z_id = Convert.ToInt32(collection["bitis_tarihi"]);
                        projeEntities1 db2 = new projeEntities1();
                        var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                        if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                        {
                            bitis_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                        }
                        if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                        {
                            bitis_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                        }
                    }
                }

                if(check_bitNull != 1)
                    urun.bitis_tarihi = Convert.ToDateTime(bitis_tarihi);

                urun.kategori_id = Convert.ToInt32(collection["kategori"]);
                urun.durum = collection["durum"];
                urun.yayin_durumu = collection["ydurum"];
                urun.teklif_durumu = collection["tdurum"];
                urun.description = collection["description"];
                urun.keywords = collection["keywords"];
                urun.aciklama = collection["aciklama"];
                urun.tarih = DateTime.Now;

                db.SaveChanges();

                TempData["UrunDuzenlemeSonucu"] = collection["adi"] + " adlı ürünü güncellediniz.";

                return RedirectToAction("FirmaUrunleri", "Urunler");
            }
            catch
            {
                return RedirectToAction("FirmaUrunleri", "Urunler");
            }
        }

        public ActionResult FirmaUrunSil(int id)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                var urun = (from u in db.urunler where u.id == id && (u.uyeler.yetki == "Admin" || u.uyeler.yetki == "Yönetici") select u).First();

                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(x => x.urun_id == urun.id));

                db.teklifler.RemoveRange(db.teklifler.Where(x => x.urun_id == urun.id));

                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(x => x.urun_id == urun.id));

                db.kargolar.RemoveRange(db.kargolar.Where(x => x.urun_id == urun.id));

                string resim_yol = "~/Assets/uploads/products_img/" + urun.resim;
                if (System.IO.File.Exists(Server.MapPath(resim_yol)))
                {
                    System.IO.File.Delete(Server.MapPath(resim_yol));
                }
                db.urunler.Remove(urun);
                db.SaveChanges();

                TempData["UrunSilmeSonucu"] = urun.adi + " adlı ürünü sildiniz.";

                return RedirectToAction("FirmaUrunleri", "Urunler");
            }
            catch
            {
                return RedirectToAction("FirmaUrunleri", "Urunler");
            }
        }

        public ActionResult SaticiUrunleri()
        {

            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "SaticiUrunleri";

            if (TempData["UrunDuzenlemeSonucu"] != null)
            {
                ViewBag.UrunDuzenlemeSonucu = TempData["UrunDuzenlemeSonucu"].ToString();
            }
            if (TempData["UrunSilmeSonucu"] != null)
            {
                ViewBag.UrunSilmeSonucu = TempData["UrunSilmeSonucu"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            var satici_urunler = from u in db.urunler where u.onay_durumu == "Evet" && u.uyeler.yetki == "Üye" select u;

            return View(satici_urunler.ToList().OrderBy(x=>x.durum).ThenBy(t=>t.tarih));
        }

        public ActionResult SaticiUrunlerDetay(int id)
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "SaticiUrunleri";

            projeEntities1 db = new projeEntities1();
            var urun = from u in db.urunler where u.id == id && u.uyeler.yetki == "Üye" select u;

            return View(urun.First());
        }

        public ActionResult SaticiUrunlerDuzenle(int id)
        {
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "SaticiUrunleri";
            projeEntities1 db = new projeEntities1();

            var urun = from u in db.urunler where u.id == id && u.uyeler.yetki == "Üye" select u;
            ViewBag.Urun = urun.First();

            var kategoriler = from u in db.kategoriler select u;
            ViewBag.Kategoriler = kategoriler.ToList();

            var zamanlar = from u in db.zamanlar select u;
            return View(zamanlar.ToList().OrderByDescending(x => x.zaman_dilimi).ThenBy(x => Convert.ToInt32(x.zaman)));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaticiUrunlerDuzenle(int id, FormCollection collection)
        {
            try
            {
                int check_bitNull = 0, check_basNull = 0;
                projeEntities1 db = new projeEntities1();
                var urun = (from u in db.urunler where u.id == id select u).First();

                urun.adi = collection["adi"];
                urun.stok = 1;
                urun.taban_fiyati = Convert.ToDecimal(collection["taban_fiyati"]);
                urun.tavan_fiyati = Convert.ToDecimal(collection["tavan_fiyati"]);

                DateTime baslangic_tarihi = DateTime.Now, bitis_tarihi = DateTime.Now;

                if (Convert.ToInt32(collection["baslangic_tarihi"]) == -1)
                {
                    if (collection["bas_tarihi"] != "")
                        baslangic_tarihi = Convert.ToDateTime(collection["bas_tarihi"]);
                    else
                        check_basNull = 1;
                }
                else
                {
                    if (Convert.ToInt32(collection["baslangic_tarihi"]) != 0)
                    {
                        int z_id = Convert.ToInt32(collection["baslangic_tarihi"]);
                        projeEntities1 db2 = new projeEntities1();
                        var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                        if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                        {
                            baslangic_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                        }
                        if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                        {
                            baslangic_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                        }
                    }
                }

                if (check_basNull != 1)
                    urun.baslangic_tarihi = Convert.ToDateTime(baslangic_tarihi);


                if (Convert.ToInt32(collection["bitis_tarihi"]) == -1)
                {
                    if (collection["bit_tarihi"] != "")
                        bitis_tarihi = Convert.ToDateTime(collection["bit_tarihi"]);
                    else
                        check_bitNull = 1;
                }
                else
                {
                    if (Convert.ToInt32(collection["bitis_tarihi"]) != 0)
                    {
                        int z_id = Convert.ToInt32(collection["bitis_tarihi"]);
                        projeEntities1 db2 = new projeEntities1();
                        var zaman = (from z in db2.zamanlar where z.id == z_id select z).First();

                        if (zaman.zaman_dilimi.Trim().Equals("Saat"))
                        {
                            bitis_tarihi = baslangic_tarihi.AddHours(Convert.ToDouble(zaman.zaman));
                        }
                        if (zaman.zaman_dilimi.Trim().Equals("Gün"))
                        {
                            bitis_tarihi = baslangic_tarihi.AddDays(Convert.ToDouble(zaman.zaman));
                        }
                    }
                }

                if (check_bitNull != 1)
                    urun.bitis_tarihi = Convert.ToDateTime(bitis_tarihi);

                urun.kategori_id = Convert.ToInt32(collection["kategori"]);
                urun.durum = collection["durum"];
                urun.yayin_durumu = collection["ydurum"];
                urun.teklif_durumu = collection["tdurum"];
                urun.onay_durumu = collection["odurum"];
                urun.description = collection["description"];
                urun.keywords = collection["keywords"];
                urun.aciklama = collection["aciklama"];
                urun.tarih = DateTime.Now;

                db.SaveChanges();

                TempData["UrunDuzenlemeSonucu"] = collection["adi"] + " adlı ürünü güncellediniz.";

                return RedirectToAction("SaticiUrunleri", "Urunler");
            }
            catch
            {
                return RedirectToAction("SaticiUrunleri", "Urunler");
            }
        }

        public ActionResult SaticiUrunSil(int id)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                var urun = (from u in db.urunler where u.id == id && u.uyeler.yetki == "Üye" select u).First();

                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(x => x.urun_id == urun.id));

                db.teklifler.RemoveRange(db.teklifler.Where(x => x.urun_id == urun.id));

                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(x => x.urun_id == urun.id));

                db.kargolar.RemoveRange(db.kargolar.Where(x => x.urun_id == urun.id));

                string resim_yol = "~/Assets/uploads/products_img/" + urun.resim;
                if (System.IO.File.Exists(Server.MapPath(resim_yol)))
                {
                    System.IO.File.Delete(Server.MapPath(resim_yol));
                }
                db.urunler.Remove(urun);
                db.SaveChanges();

                TempData["UrunSilmeSonucu"] = urun.adi + " adlı ürünü sildiniz.";

                return RedirectToAction("SaticiUrunleri", "Urunler");
            }
            catch
            {
                return RedirectToAction("SaticiUrunleri", "Urunler");
            }
        }

        public ActionResult OnayBekleyenler()
        {
            if (TempData["UrunOnay"] != null)
            {
                ViewBag.UrunOnay = TempData["UrunOnay"].ToString();
            }

            if (TempData["RedSilme"] != null)
            {
                ViewBag.RedSilme = TempData["RedSilme"].ToString();
            }

            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "OnayBekleyenler";
            projeEntities1 db = new projeEntities1();

            return View(db.urunler.ToList().Where(x=>x.onay_durumu == "Hayır").OrderByDescending(y=>y.tarih));
        }

        public ActionResult Onay_urun_islem(int urun_id)
        {
           
            ViewBag.ActiveMenu = "Urunler";
            ViewBag.ActiveSubMenu = "OnayBekleyenler";
            projeEntities1 db = new projeEntities1();
            var urun = from u in db.urunler where u.id == urun_id select u;
            return View(urun.First());
        }

        [HttpPost]
        public ActionResult Onay_urun_islem(FormCollection col,int urun_id)
        {
            var sec = Convert.ToInt32(col["islem_sec"]);
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == urun_id && u.onay_durumu == "Hayır" select u).First();

            if (sec == 1)
            {
                urun.onay_durumu = "Evet";
                urun.durum = "Aktif";
                db.SaveChanges();
                TempData["UrunOnay"] = urun.adi + " isimli ürün onaylandı ve ürünler listesi sayfasına taşındı.";

            }
            else if(sec == 2)
            {
                string resim_yol = "~/Assets/uploads/products_img/" + urun.resim;
                if (System.IO.File.Exists(Server.MapPath(resim_yol)))
                {
                    System.IO.File.Delete(Server.MapPath(resim_yol));
                }
                db.urunler.Remove(urun);
                db.SaveChanges();

                TempData["RedSilme"] = urun.adi + " adlı ürünü reddederek sildiniz.";
            }
            
            return RedirectToAction("OnayBekleyenler","Urunler");
        }
    }
}
