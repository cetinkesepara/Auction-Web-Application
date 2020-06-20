using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proje.Areas.admin.Models
{
    public class myfuctions
    {

        public string sameNameImageChange(string imageName)
        {
            projeEntities1 db = new projeEntities1();
            var resim_adlari = (from r in db.urunler select r.resim).ToList();

            int index = 0;
            int img_count = 1;
            int count = resim_adlari.Count();
            string imageNameTemp = imageName;

            if( count > 0)
            {
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
            }
            
            return imageName;
        }
    }
}