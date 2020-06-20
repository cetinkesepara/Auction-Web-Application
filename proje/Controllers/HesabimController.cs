using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;
using proje.Models;

namespace proje.Controllers
{
    [GirisyapAuthorize]
    public class HesabimController : Controller
    {
        // GET: Hesabim
        public ActionResult Index()
        {
            if (TempData["ProfilResimYuklemeSonucu"] != null)
            {
                ViewBag.ProfilResimYuklemeSonucu = TempData["ProfilResimYuklemeSonucu"].ToString();
            }
            if (TempData["ResimTuruHata"] != null)
            {
                ViewBag.ResimTuruHata = TempData["ResimTuruHata"].ToString();
            }

            if (TempData["HesabimGuncelle"] != null)
            {
                ViewBag.HesabimGuncelle = TempData["HesabimGuncelle"].ToString();
            }

            if (TempData["bakiye_yukle"] != null)
            {
                ViewBag.bakiye_yukle = TempData["bakiye_yukle"].ToString();
            }

            if (TempData["bakiye_yukle_hata"] != null)
            {
                ViewBag.bakiye_yukle_hata = TempData["bakiye_yukle_hata"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            var uye_id = Convert.ToInt32(Session["uye_id"]);
            var uye = from u in db.uyeler where u.id == uye_id select u;
            return View(uye.First());
        }

        public ActionResult Urun_ekle()
        {
            if (TempData["ResimTuruHata"] != null)
            {
                ViewBag.ResimTuruHata = TempData["ResimTuruHata"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            var kategoriler = from u in db.kategoriler select u;
            ViewBag.Kategoriler = kategoriler.ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Urun_ekle(FormCollection collection, HttpPostedFileBase imageFile)
        {
           
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    if (imageFile.ContentType == "image/jpeg" || imageFile.ContentType == "image/png")
                    {

                        
                        myfuctions func = new myfuctions(); 
                        string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName);
                        imageName = func.sameNameImageChange(imageName);

                    string path = System.IO.Path.Combine(Server.MapPath("~/Assets/uploads/products_img"), imageName+".jpg");
                        imageFile.SaveAs(path);

                        var urunModel = new urunler();
                        urunModel.resim = imageName + ".jpg";
                        urunModel.adi = collection["adi"];
                        urunModel.stok = 1;
                        urunModel.taban_fiyati = Convert.ToDecimal(collection["taban_fiyati"]);
                        urunModel.tavan_fiyati = Convert.ToDecimal(collection["tavan_fiyati"]);
                        urunModel.kategori_id = Convert.ToInt32(collection["kategori"]);
                        urunModel.durum = "Pasif";
                        urunModel.description = collection["description"];
                        urunModel.keywords = collection["keywords"];
                        urunModel.aciklama = collection["aciklama"];
                        urunModel.tarih = DateTime.Now;
                        urunModel.uye_id = Convert.ToInt32(Session["uye_id"]);
                        urunModel.yayin_durumu = "Hayır";
                        urunModel.teklif_durumu = "Hayır";
                        urunModel.onay_durumu = "Hayır";

                        projeEntities1 db = new projeEntities1();
                        db.urunler.Add(urunModel);
                        db.SaveChanges();

                        /* Yeni Sistem
                        int last_id = db.urunler.Max(x => x.id);
                        var kargoModel = new kargolar();
                        kargoModel.tur = "Alınan";
                        kargoModel.durum = "Ürün Onayı Bekleniyor";
                        kargoModel.tarih = DateTime.Now;
                        kargoModel.uye_id = Convert.ToInt32(Session["uye_id"]);
                        kargoModel.urun_id = last_id;
                        db.kargolar.Add(kargoModel);
                        db.SaveChanges();
                        */

                        TempData["UrunEklemeSonucu"] = collection["adi"] + " adlı ürünü eklediniz.";

                        return RedirectToAction("Urun_ekle_sonuc", "Hesabim");

                    }
                    else
                    {
                        TempData["ResimTuruHata"] = "Sadece jpeg ve png türündeki resimleri yükleyebilirsiniz";
                        return RedirectToAction("Urun_ekle", "Hesabim");
                    }

                }

                return RedirectToAction("Urun_ekle", "Hesabim");
         
        }

        public ActionResult Urun_ekle_sonuc()
        {
            ViewBag.UrunEklemeSonucu = "Ürününüz eklenmiştir. Ürün bilgileri onaylandığında aktif hale getirilecektir. Onay bekleyenler sayfasından takip edebilirsiniz.";

            return View();
        }

        public ActionResult Urun_listeleme()
        {
            if (TempData["IzinsizDizin"] != null)
            {
                ViewBag.IzinsizDizin = TempData["IzinsizDizin"];
            }

            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var urunler = from u in db.urunler where u.uye_id == uye_id && u.onay_durumu == "Evet" select u;

            return View(urunler.ToList());
        }

        public ActionResult Urun_detay(int urun_id)
        {
            try
            {
                if (TempData["YayinDurumu"] != null)
                {
                    ViewBag.YayinDurumu = TempData["YayinDurumu"];
                }

                if (TempData["UrunDuzenlemeSonucu"] != null)
                {
                    ViewBag.UrunDuzenlemeSonucu = TempData["UrunDuzenlemeSonucu"].ToString();
                }

                projeEntities1 db = new projeEntities1();
                int uye_id = Convert.ToInt32(Session["uye_id"]);
                var urun = from u in db.urunler where u.id == urun_id && u.uye_id == uye_id select u;

                return View(urun.First());
            }
            catch
            {
                TempData["IzinsizDizin"] = "Giriş izninizin olmadığı bir dizine ulaşmaya çalıştınız!";
                return RedirectToAction("Urun_listeleme", "Hesabim");
            }
        }

        public ActionResult Urun_duzenle(int urun_id)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                int uye_id = Convert.ToInt32(Session["uye_id"]);
                var urun = from u in db.urunler where u.id == urun_id && u.uye_id == uye_id && u.onay_durumu == "Evet" select u;
                var kategoriler = from k in db.kategoriler select k;
                ViewBag.Kategoriler = kategoriler.ToList();

                return View(urun.First());
            }
            catch
            {
                TempData["IzinsizDizin"] = "Giriş izninizin olmadığı bir dizine ulaşmaya çalıştınız!";
                return RedirectToAction("Urun_listeleme", "Hesabim");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Urun_duzenle(FormCollection collection, int urun_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var urun = (from u in db.urunler where u.id == urun_id && u.uye_id == uye_id select u).First();

            if(urun.yayin_durumu == "Evet")
            {
                TempData["YayinDurumu"] = "Yayında olan ürünler düzenlenemez!";
                return RedirectToAction("Urun_detay", "Hesabim",new { urun_id = urun_id });
            }
            else
            {
                urun.adi = collection["adi"];
                urun.kategori_id = Convert.ToInt32(collection["kategori"]);
                urun.stok = 1;
                urun.taban_fiyati = Convert.ToDecimal(collection["taban_fiyati"]);
                urun.tavan_fiyati = Convert.ToDecimal(collection["tavan_fiyati"]);
                urun.description = collection["description"];
                urun.keywords = collection["keywords"];
                urun.aciklama = collection["aciklama"];
                urun.tarih = DateTime.Now;
                db.SaveChanges();

                TempData["UrunDuzenlemeSonucu"] = collection["adi"] + " adlı ürünü güncellediniz.";

                return RedirectToAction("Urun_detay", "Hesabim", new { urun_id = urun_id });
            }
        }

        public ActionResult Urun_islem(int urun_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var zamanlar = from u in db.zamanlar select u;
            var teklif_butonlari = from u in db.teklif_butonlari select u;
            var urun = from u in db.urunler where u.id == urun_id && u.uye_id == uye_id && u.yayin_durumu == "Hayır" && u.onay_durumu == "Evet" select u;

            if (urun.Count() <= 0)
            {
                return RedirectToAction("Urun_listeleme", "Hesabim");
            }

            ViewBag.urun = urun.First();
            ViewBag.teklif_butonlari = teklif_butonlari.ToList().OrderBy(x=>x.miktar);    

            return View(zamanlar.ToList().OrderByDescending(x => x.zaman_dilimi).ThenBy(x => Convert.ToInt32(x.zaman)));
        }

        [HttpPost]
        public ActionResult Urun_islem(FormCollection collection, int urun_id)
        {
            projeEntities1 db = new projeEntities1();
            var urun = (from u in db.urunler where u.id == urun_id select u).First();

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

            foreach(var item in button)
            {
                for(int c = 0; c < colCount-2; c++)
                {
                    if(item.id == Convert.ToInt32(collection[c]))
                    {
                        urun_button.urun_id = urun_id;
                        urun_button.teklif_butonu_id = item.id;
                        db.urun_teklif_butonlari.Add(urun_button);
                        db.SaveChanges();
                    }
                }
            }




            return RedirectToAction("Urun_listeleme", "Hesabim");
        }

        public ActionResult Onay_bekleyenler()
        {
            projeEntities1 db = new projeEntities1();
            var uye_id = Convert.ToInt32(Session["uye_id"]);
            var urunler = from u in db.urunler where u.uye_id == uye_id && u.onay_durumu == "Hayır" select u;
            return View(urunler.ToList().OrderByDescending(x=>x.tarih));
        }

        public ActionResult Gonderilen_kargolar()
        {
            if(TempData["IzinsizDizin"] != null)
            {
                ViewBag.IzinsizDizin = TempData["IzinsizDizin"];
            }

            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = from k in db.kargolar where k.urunler.uye_id == uye_id && k.tur == "Satıcı-Üye" select k;
            return View(kargo.ToList().OrderByDescending(x => x.tarih));
        }

        public ActionResult Kargo_gonder_islem(int kargo_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = from k in db.kargolar where k.id == kargo_id && k.urunler.uye_id == uye_id && k.tur == "Satıcı-Üye" select k;

            return View(kargo.First());
        }

        [HttpPost]
        public ActionResult Kargo_gonder_islem(FormCollection col,int kargo_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = (from k in db.kargolar where k.id == kargo_id && k.urunler.uye_id == uye_id && k.tur == "Satıcı-Üye" select k).First();

            if(Convert.ToInt32(col["kargo_islem"]) == 1)
            {
                kargo.durum = "Kargo Yola Çıktı";
                db.SaveChanges();
            }

            return RedirectToAction("Gonderilen_kargolar", "Hesabim");
        }



        public ActionResult Alinan_kargolar()
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = from k in db.kargolar where k.uye_id == uye_id select k;
            return View(kargo.ToList().OrderByDescending(x => x.tarih));
        }

        public ActionResult Kargo_alinan_islem(int kargo_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = from k in db.kargolar where k.id == kargo_id && k.uye_id == uye_id select k;

            return View(kargo.First());
        }

        [HttpPost]
        public ActionResult Kargo_alinan_islem(FormCollection col,int kargo_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var kargo = (from k in db.kargolar where k.id == kargo_id && k.uye_id == uye_id select k).First();

            if (Convert.ToInt32(col["kargo_islem"]) == 1)
            {
                kargo.durum = "Kargo Teslim Edildi";
                db.SaveChanges();
            }

            return RedirectToAction("Alinan_kargolar", "Hesabim");
        }

        public ActionResult Alinan_mesajlar()
        {
            if (TempData["IzinsizMesajDizin"] != null)
            {
                ViewBag.IzinsizMesajDizin = TempData["IzinsizMesajDizin"];
            }
            if (TempData["AlinanMesajSilme"] != null)
            {
                ViewBag.AlinanMesajSilme = TempData["AlinanMesajSilme"];
            }

            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var mesajlar = from m in db.mesajlasmalar where m.uye_id == uye_id && m.tur == "Alınan" select m;
            return View(mesajlar.ToList().OrderByDescending(x=>x.durum).ThenBy(t=>t.tarih));
        }

        public ActionResult Mesaj_alinan_oku(int mesaj_id)
        {
            try
            {
                projeEntities1 db = new projeEntities1();
                int uye_id = Convert.ToInt32(Session["uye_id"]);
                var mesaj = (from m in db.mesajlasmalar where m.id == mesaj_id && m.uye_id == uye_id select m).First();
                mesaj.durum = "Okundu";
                db.SaveChanges();

                return View(mesaj);
            }
            catch
            {
                TempData["IzinsizMesajDizin"] = "Giriş izninizin olmadığı bir dizine ulaşmaya çalıştınız!";
                return RedirectToAction("Alinan_mesajlar", "Hesabim");
            }
            
        }

        public ActionResult Mesaj_alinan_sil(int mesaj_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var mesaj = (from m in db.mesajlasmalar where m.id == mesaj_id && m.uye_id == uye_id select m).First();
           
            TempData["AlinanMesajSilme"] = mesaj.tarih + " tarihli mesajı sildiniz.";
            db.mesajlasmalar.Remove(mesaj);
            db.SaveChanges();

            return RedirectToAction("Alinan_mesajlar", "Hesabim");
        }


        [HttpPost]
        public ActionResult Profil_resmi_yukle(HttpPostedFileBase imageFile)
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
                        int sess_id = Convert.ToInt32(Session["uye_id"]);
                        var uye = (from u in db.uyeler where u.id == sess_id select u).First();
                        string resim_yol = "~/Assets/uploads/profile_img/" + uye.resim;
                        if (System.IO.File.Exists(Server.MapPath(resim_yol)) && uye.resim != "default_profile.jpg")
                        {
                            System.IO.File.Delete(Server.MapPath(resim_yol));
                        }

                        uye.resim = resim_adi;

                        db.SaveChanges();

                        TempData["ProfilResimYuklemeSonucu"] = resim_adi + " adlı resim yüklendi.";

                        return RedirectToAction("Index", "Hesabim");
                    }
                    else
                    {
                        TempData["ResimTuruHata"] = "Sadece jpeg ve png türündeki resimleri yükleyebilirsiniz";
                        return RedirectToAction("Index", "Hesabim");
                    }

                }

                return RedirectToAction("Index", "Hesabim");
            }
            catch
            {
                return RedirectToAction("Index", "Hesabim");
            }

        }

        [HttpPost]
        public ActionResult Hesabim_duzenle(FormCollection col)
        {
            var adsoy = col["adsoy"];
            var telefon = col["telefon"];
            var sehir = col["sehir"];
            var adres = col["adres"];
            var uye_id = Convert.ToInt32(Session["uye_id"]);

            projeEntities1 db = new projeEntities1();
            var uye = (from u in db.uyeler where u.id == uye_id select u).First();
            uye.adsoy = adsoy;
            uye.telefon = telefon;
            uye.sehir = sehir;
            uye.adres = adres;
            db.SaveChanges();

            TempData["HesabimGuncelle"] = "Bilgiler guncellendi";

            return RedirectToAction("Index", "Hesabim");
        }

        public ActionResult Bakiye_yukle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Bakiye_yukle(FormCollection col)
        {
            projeEntities1 db = new projeEntities1();
            try
            {
                var bakiye_yukle = new bakiye_yukle();
                bakiye_yukle.miktar = Convert.ToDouble(col["tl"]);
                bakiye_yukle.uye_id = Convert.ToInt32(Session["uye_id"]);
                bakiye_yukle.tarih = DateTime.Now;
                bakiye_yukle.durum = "Onay Bekleniyor";
                db.bakiye_yukle.Add(bakiye_yukle);
                db.SaveChanges();

                TempData["bakiye_yukle"] = "Bakiye yükleme talebiniz alınmıştır. Hesabınızdaki yüklemelerim sayfasından işlemi takip edebilirsiniz.";
            }
            catch
            {
                TempData["bakiye_yukle_hata"] = "İşlem gerçekleştirilemedi.";
            }

            return RedirectToAction("Index","Hesabim");
        }

        public ActionResult Yuklemelerim()
        {
            if (TempData["yukleme_sil"] != null)
            {
                ViewBag.yukleme_sil = TempData["yukleme_sil"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            return View(db.bakiye_yukle.ToList().Where(x=>x.uye_id == uye_id).OrderByDescending(t=>t.tarih));
        }

        public ActionResult Yuklemelerim_sil(int b_id)
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var by = db.bakiye_yukle.Where(x => x.id == b_id).Where(x=>x.uye_id == uye_id).First();

            if(by.durum == "Onaylandı" || by.durum == "Reddedildi")
            {
                db.bakiye_yukle.RemoveRange(db.bakiye_yukle.Where(x => x.id == b_id));
                db.SaveChanges();

                TempData["yukleme_sil"] = "Yukleme bilgisi silindi.";
            }

            return RedirectToAction("Yuklemelerim","Hesabim");
        }

        public ActionResult Hesap_ayarlari()
        {
            if (TempData["yeni_sifre"] != null)
            {
                ViewBag.yeni_sifre = TempData["yeni_sifre"].ToString();
            }

            if (TempData["eski_sifre_hata"] != null)
            {
                ViewBag.eski_sifre_hata = TempData["eski_sifre_hata"].ToString();
            }
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);
            var uye = from u in db.uyeler where u.id == uye_id select u;
            return View(uye.First());
        }

        [HttpPost]
        public ActionResult Hesap_sifre_degis(FormCollection col)
        {
            var eski_sifre = col["eski_sifre"];
            var yeni_sifre = col["yeni_sifre"];
            var yeni_sifre_tekrar = col["yeni_sifre_tekrar"];

            if (yeni_sifre == yeni_sifre_tekrar)
            {
                projeEntities1 db = new projeEntities1();
                int uye_id = Convert.ToInt32(Session["uye_id"]);
                var uye = from u in db.uyeler where u.id == uye_id && u.sifre == eski_sifre select u;

                if (uye.ToList().Count() == 1)
                {
                    var u = uye.First();      
                    u.sifre = yeni_sifre;
                    db.SaveChanges();

                    TempData["yeni_sifre"] = "Şifreniz değiştirildi.";

                }
                else
                {
                    TempData["eski_sifre_hata"] = "Girilen eski şifre hatalı";

                }
            }


            return RedirectToAction("Hesap_ayarlari","Hesabim");
        }

        public ActionResult Tekliflerim()
        {
            projeEntities1 db = new projeEntities1();
            int uye_id = Convert.ToInt32(Session["uye_id"]);

            ViewBag.Teklifler = from gr in db.teklif_miktarlari
                                where gr.uye_id == uye_id
                                group gr by new { gr.uye_id, gr.urunler.bitis_tarihi, gr.urunler.adi, gr.urun_id }
                                into g
                                select new tekliflerimList { uye_id = g.Key.uye_id, urun_adi = g.Key.adi, bitis_tarihi = g.Key.bitis_tarihi , urun_id = g.Key.urun_id, toplam = g.Sum(a => a.teklif_butonlari.miktar) };

            return View();
        }
    }
}