
$(document).ready(function () {
    var urun_id = $('#urun_id').val();
    bakiyeYenile(urun_id);
    
    setInterval(function () { Teklifler(urun_id); }, 2000);
});

function bakiyeYenile(urun_id) {
    var uye_id = $('#session').val();

    $.ajax({
        url: "/Anasayfa/BakiyeGetir",
        data:{
            uye_id: uye_id,
            urun_id: urun_id
        },
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '', max='';
            $.each(result, function (key, item) {
                if (key == 0) {
                    html += item;
                    html += " TL";
                }
                else if (key == 1) {
                    max += item;
                    max += " TL";
                }
            });

            $('#bakiye').html(html);
            $('#toplamTeklif').html(max);
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function Teklifler(urun_id) {

    $.ajax({
        url: "/Anasayfa/Teklifler",
        data: {
            urun_id: urun_id
        },
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            
            var htmlMax = '', html = '', htmlToplam = '';
            //Max Teklif
            htmlMax += result.data2;
            htmlMax += " TL";
            
            //Anlık teklifler
            $.each(result.data1, function (key, item) {
                if (key == 0) {
                    html += '<li>' + item.adsoy + ' ' + item.miktar + ' TL <span class="blueSpan">Yeni</span></li>';
                }
                else {
                    html += '<li>' + item.adsoy + ' ' + item.miktar + ' TL</li>';
                }
                
            });

            //Anlık Toplam Teklifler
            $.each(result.data3, function (key, item) {
                if (key == 0) {
                    htmlToplam += '<li>' + item.adsoy + ' ' + item.toplamTeklif + ' TL <span class="blueSpan">Kazanıyor</span></li>';
                }
                else {
                    htmlToplam += '<li>' + item.adsoy + ' ' + item.toplamTeklif + ' TL</li>';
                }

            });

            $('#toplam_teklifler_listesi').html(htmlToplam);
            $('#maxTeklif').html(htmlMax);
            $('#teklifler_listesi').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function AuctionAdd(btn_id, urun_id, uye_id) {

    var aucObj = {
        teklif_butonu_id: btn_id,
        urun_id: urun_id,
        uye_id: uye_id
    };

    $.ajax({
        url: "/Anasayfa/AuctionAdd",
        data: JSON.stringify(aucObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result == 100) {
                alert("BAKİYENİZ TEKLİF VERMEK İÇİN YETERLİ DEĞİL");
                return false;
            }

            bakiyeYenile(urun_id);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    });
}
