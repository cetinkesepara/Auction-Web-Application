using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using proje.Areas.admin.Models;

namespace proje.Models
{
    public class AuctionDB
    {
        private projeEntities1 db = null;
        public AuctionDB()
        {
            db = new projeEntities1();
        }

        public List<double> BakiyeGetir(int uye_id,int urun_id)
        {
            List<double> lst = new List<double>();
            double teklif_toplam;
            var uye = (from u in db.uyeler where u.id == uye_id select u).First();
            var teklif = (from t in db.teklif_miktarlari where t.uye_id == uye_id && t.urun_id == urun_id select t.teklif_butonlari.miktar).ToList(); 
            if(teklif.Count() > 0)
            {
                teklif_toplam = teklif.Sum();
            }
            else
            {
                teklif_toplam = 0;
            }

            lst.Add(uye.bakiye);
            lst.Add(teklif_toplam);

            //return uye.bakiye;
            return lst;
        }

        public List<tekliflerListesi> Teklifler(int urun_id)
        {
            List<tekliflerListesi> teklifList = new List<tekliflerListesi>();

            var teklif_mik = (from t in db.teklif_miktarlari where t.urun_id == urun_id select t).ToList().OrderByDescending(x => x.tarih);
            foreach(var item in teklif_mik)
            {
                teklifList.Add(new tekliflerListesi {
                    adsoy = item.uyeler.adsoy,
                    miktar = item.teklif_butonlari.miktar
                });

            }

            return teklifList;
        }

        public List<toplamTekliflerListesi> ToplamTeklifler(int urun_id)
        {
            List<toplamTekliflerListesi> toplamList = new List<toplamTekliflerListesi>();
            /*var group = db.teklif_miktarlari
                .Where(a => a.urun_id == urun_id)
                .GroupBy(a => a.uye_id)
                .Select(a => new { Toplam = a.Sum(b => b.teklif_butonlari.miktar), ID = a.Key, Adi = a.Key })
                .OrderByDescending(x => x.Toplam);*/

            var group = (from gr in db.teklif_miktarlari
                         where gr.urun_id == urun_id
                         group gr by new { gr.uye_id, gr.uyeler.adsoy }
                         into g
                         select new { g.Key.uye_id, g.Key.adsoy, Toplam = g.Sum(a => a.teklif_butonlari.miktar) }).ToList().OrderByDescending(x => x.Toplam);

            foreach(var item in group)
            {
                toplamList.Add(new toplamTekliflerListesi {
                    adsoy = item.adsoy,
                    toplamTeklif = item.Toplam
                });
            }

            return toplamList;

        }

        public double MaxTeklif(int urun_id)
        {
           
            var group = db.teklif_miktarlari
                .Where(a => a.urun_id == urun_id)
                .GroupBy(a => a.uye_id)
                .Select(a => new { Toplam = a.Sum(b => b.teklif_butonlari.miktar), ID = a.Key });

            int eb = 0;
            foreach (var item in group)
            {
                if (item.Toplam > eb)
                {
                    eb = item.Toplam;
                }
            }

            return eb;
        }

        public int AuctionAdd(teklif_miktarlari auc)
        {
            int i,c;
            var aucCount = new teklif_miktarlari();
            var btn = (from b in db.teklif_butonlari where b.id == auc.teklif_butonu_id select b).First();
            var uye = (from u in db.uyeler where u.id == auc.uye_id select u).First();

            if (Convert.ToDouble(btn.miktar) > uye.bakiye)
            {
                return 100;
            }

            aucCount.teklif_butonu_id = auc.teklif_butonu_id;
            aucCount.urun_id = auc.urun_id;
            aucCount.uye_id = auc.uye_id;
            aucCount.tarih = DateTime.Now;

            db.teklif_miktarlari.Add(aucCount);
            i = db.SaveChanges();

            if (Convert.ToBoolean(i))
            {
                
                var urun = (from ur in db.urunler where ur.id == auc.urun_id select ur).First();
                

                uye.bakiye -= Convert.ToDouble(btn.miktar);
                c = db.SaveChanges();

                if (Convert.ToBoolean(c))
                {
                    var group = db.teklif_miktarlari
                        .Where(a => a.urun_id == auc.urun_id)
                        .GroupBy(a => a.uye_id)
                        .Select(a => new { Toplam = a.Sum(b => b.teklif_butonlari.miktar), ID = a.Key });

                    int eb = 0;
                    int max_uye_id = 0;
                    foreach (var item in group)
                    {
                        if (item.Toplam > eb)
                        {
                            eb = item.Toplam;
                            max_uye_id = item.ID;
                        }
                    }

                    var teklif = from t in db.teklifler where t.urun_id == auc.urun_id select t;
                    if (teklif.Count() > 0)
                    {
                        var tek = teklif.First();
                        tek.uye_id = max_uye_id;
                        tek.teklif = eb;
                        db.SaveChanges();
                    }
                    else
                    {
                        var teklifler = new teklifler();
                        teklifler.urun_id = auc.urun_id;
                        teklifler.uye_id = max_uye_id;
                        teklifler.teklif = eb;
                        teklifler.durum = "Yok";
                        db.teklifler.Add(teklifler);
                        db.SaveChanges();
                    
                    }
                }
            }

            return i;
        }

        public List<urunler> GetProducts(int p_start,int p_finish, List<string> catIDList, List<string> autTimeList, double min, double max)
        {
            List<urunler> p_list = new List<urunler>();
            IEnumerable<urunler> urunler;
            //var urunler = (from u in db.urunler where u.durum == "Aktif" && u.yayin_durumu == "Evet" select u).ToList();
            // var p_urunler = urunler.OrderBy(x => x.teklif_durumu).ThenBy(y => y.tarih).ToArray();

            if(catIDList[0] != "[]")
            {
                string[] catIds = new string[db.kategoriler.Count()];
                catIds = catIDList[0].Split('\"');
                int[] catList = new int[db.kategoriler.Count()];


                int s = 0;
                for (int i = 0; i < catIds.Count(); i++)
                {
                    if (i % 2 == 1)
                    {
                        catList[s] = Convert.ToInt32(catIds[i]);
                        s++;
                    }
                }

                 urunler = (from u in db.urunler
                               where u.durum == "Aktif" && u.yayin_durumu == "Evet"
                               select u).ToList().Where(r => catList.Contains(r.kategori_id));
                

            }
            else
            {
                 urunler = (from u in db.urunler where u.durum == "Aktif" && u.yayin_durumu == "Evet" select u).ToList();
            }

            if(min > 0)
            {
                urunler = urunler.Where(x => x.taban_fiyati > Convert.ToDecimal(min));
            }

            if(max > 0)
            {
                urunler = urunler.Where(x => x.tavan_fiyati < Convert.ToDecimal(max));
            }

            string[] autL = new string[3];
            autL = autTimeList[0].Split('\"');
            int[] autList = new int[3];

            if (autTimeList[0] != "[]")
            {
                int a = 0;
                for (int i = 0; i < autL.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        autList[a] = Convert.ToInt32(autL[i]);
                        a++;
                    }
                }

            }



            DateTime localTime = DateTime.Now;
    
            List<urunler> p_urunler = new List<Areas.admin.Models.urunler>();
            List<urunler> u1 = new List<Areas.admin.Models.urunler>();
            List<urunler> u2 = new List<Areas.admin.Models.urunler>();
            List<urunler> u3 = new List<Areas.admin.Models.urunler>();

            foreach (var item in urunler)
            {
                if(item.baslangic_tarihi < localTime && localTime < item.bitis_tarihi)
                {
                    u1.Add(item);
                }
                else if(item.baslangic_tarihi > localTime && localTime < item.bitis_tarihi)
                {
                    u2.Add(item);
                }
                else if(item.baslangic_tarihi < localTime && localTime > item.bitis_tarihi)
                {
                    u3.Add(item);
                }
            }

            if(autTimeList[0] != "[]")
            {
                for(int i=0; i< autList.Length; i++)
                {
                    if(autList[i] == 1)
                    {
                        foreach (var item in u1)
                        {
                            p_urunler.Add(item);
                        }
                    }
                    if (autList[i] == 2)
                    {
                        foreach (var item in u2)
                        {
                            p_urunler.Add(item);
                        }
                    }
                    if (autList[i] == 3)
                    {
                        foreach (var item in u3)
                        {
                            p_urunler.Add(item);
                        }
                    }
                }

            }
            else
            {
                foreach (var item in u1)
                {
                    p_urunler.Add(item);
                }
                foreach (var item in u2)
                {
                    p_urunler.Add(item);
                }
                foreach (var item in u3)
                {
                    p_urunler.Add(item);
                }
            }


            var count = p_urunler.Count();

            for (int i = p_start; i < p_finish; i++)
            {
                if(i < count)
                {
                    p_list.Add(new urunler
                    {
                        id = p_urunler[i].id,
                        adi = p_urunler[i].adi,
                        resim = p_urunler[i].resim,
                        baslangic_tarihi = p_urunler[i].baslangic_tarihi,
                        bitis_tarihi = p_urunler[i].bitis_tarihi,
                        taban_fiyati = p_urunler[i].taban_fiyati,
                        tavan_fiyati = p_urunler[i].tavan_fiyati
                    });

                }
                
            }


            /*
            foreach (var item in p_urunler) {
                p_list.Add(new urunler {
                    id = item.id,
                    adi = item.adi,
                    resim = item.resim,
                    baslangic_tarihi = item.baslangic_tarihi,
                    bitis_tarihi = item.bitis_tarihi,
                    taban_fiyati = item.taban_fiyati,
                    tavan_fiyati = item.tavan_fiyati
                });
            }
            */

            return p_list;
        }

        public DateTime GetLocalTime()
        {
            DateTime localTime = DateTime.Now;
            return localTime;
        }

    }
}