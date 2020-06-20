$(document).ready(function () {
    getProducts();
    
    setInterval(function () {
        if ($(".font-time").css("font-size") == "12px") {
            $(".font-time").css("font-size", "14px");
        }
        else {
            $(".font-time").css("font-size", "12px");
        }
    }, 2000);
});

function getProducts() {



    var catCount = $("#catCount").val();
    var catIDList = [];
    for (i = 0; i < catCount; i++) {
        catIDList[i] = $("#cat" + i).val();
    }

    var autCount = $("#autCount").val();
    var autTimeList = [];
    for (i = 0; i < autCount; i++) {
        autTimeList[i] = $("#aut" + i).val();
    }

    var min;
    if ($("#min").length != 0) min = $("#min").val();
    else min = 0;

    
    var max = $("#max").val();
    if ($("#max").length != 0) max = $("#max").val();
    else max = 0;




    var p_start = $("#p_start").val();
    var p_finish = $("#p_finish").val();
   
    $.ajax({
        url: "/Anasayfa/GetProducts",
        data: {
            p_start: p_start,
            p_finish: p_finish,
            catIDList: JSON.stringify(catIDList),
            autTimeList: JSON.stringify(autTimeList),
            min: min,
            max: max
        },
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result.products, function (key, item) {
                html += '<div class="col-sm-6 col-md-6 col-lg-4">';
                html += '<div class="card">';
                html += '<h3>' + item.adi + '</h3>';
                html += '<img class="img-fluid" src="/Assets/uploads/products_img/' + item.resim + '" alt="img" width="237" height="175"/>';
                html += '<div class="b1">';
                html += '<div class="row no-gutters">';
                html += '<div class="col-6">';
                if (result.localtime < item.baslangic_tarihi) {
                    html += '<h3>Açılışa kalan süre</h3>';
                    html += '<div class="time font-time" data-countdown="' + parseJsonDate(item.baslangic_tarihi) + '"></div>';
                }
                else if (result.localtime >= item.baslangic_tarihi && result.localtime < item.bitis_tarihi) {
                    html += '<h3>Bitişe kalan süre</h3>';
                    html += '<div class="time font-time" data-countdown="' + parseJsonDate(item.bitis_tarihi) + '"></div>';
                }
                else if (result.localtime >= item.bitis_tarihi) {
                    html += '<h3>Süre</h3>';
                    html += '<div class="time">Bitti</div>';
                }
                html += '</div>';
                html += '<div class="col-6">';
                if (result.localtime < item.baslangic_tarihi) {
                    html += '<a class="button_gav" href="/Anasayfa/Teklif_zaman_kontrol?urun_id='+ item.id +'">';
                    html += '<i class="fa fa-2x fa-clock-o" aria-hidden="true"></i>';
                    html += '</a>';
                }
                else if (result.localtime >= item.baslangic_tarihi && result.localtime < item.bitis_tarihi) {
                    html += '<a class="button_gav" href="/Anasayfa/Teklif_zaman_kontrol?urun_id=' + item.id + '">';
                    html += '<i class="fa fa-2x fa-gavel" aria-hidden="true"></i>';
                    html += '</a>';
                }
                else if (result.localtime >= item.bitis_tarihi) {
                    html += '<a class="button_gav" href="/Anasayfa/Teklif_zaman_kontrol?urun_id=' + item.id + '">';
                    html += '<i class="fa fa-2x fa-times" aria-hidden="true"></i>';
                    html += '</a>';
                }
                html += '</div>';
                html += '</div>';
                html += '</div>';
                html += '<div class="b2">';
                html += '<div class="row no-gutters">';
                /*
                html += '<div class="col-6">';
                html += '<h3>En yüksek teklif</h3>';
                html += '<div class="count">- TL</div>';
                html += '</div>'; */
                html += '<div class="col-12">';
                html += '<div class="min"><span class="min">Min </span><span class="min_">'+item.taban_fiyati+'TL</span></div>';
                html += '<div class="max"><span class="max">Max </span><span class="max_">'+item.tavan_fiyati+'TL</span></div>';
                html += '</div>';
                html += '</div>';
                html += '</div>';
                html += '</div>';
                html += '</div>';
            });

            $('#mainproducts').append(html);

            $("#p_start").val(parseInt(p_start) + 6);
            $("#p_finish").val(parseInt(p_finish) + 6);
            jQuery(window).trigger('load');
        },
        complete: function (data) {
         
            getCountDown();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

 
}

function parseJsonDate(jsonDate) {

    var fullDate = new Date(parseInt(jsonDate.substr(6)));
    var twoDigitMonth = (fullDate.getMonth() + 1) + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
    var twoDigitSecond = fullDate.getSeconds() + ""; if (twoDigitSecond.length == 1) twoDigitSecond = "0" + twoDigitSecond;
    var currentDate = fullDate.getDate() + "." + twoDigitMonth + "." + fullDate.getFullYear() + " " + fullDate.getHours() + ":" + fullDate.getMinutes() + ":" + twoDigitSecond;

    return currentDate;
}



    
    
        
   

