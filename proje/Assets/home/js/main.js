//Filtreleme mobil slidedown
$(document).ready(function(){
    $("#filtreMobileTitle").click(function(){
        if($("#filtreMobileDiv").css('display') == 'none'){
            $("#filtreMobileDiv").slideDown(2000);
        }
        else{
            $("#filtreMobileDiv").slideUp(2000);
        }
                
    });
});

//Slider
$(document).ready(function(){
    $('.slider').bxSlider();
});


//Login İşlemleri
$(document).ready(function(){
    $(".l2 a").click(function(){
        $(".uyeKayit").slideDown(2000);
        $(".uyeGirisi").slideUp(2000);
    });
});
$(document).ready(function(){
    $(".uyeKayitKapat").click(function(){
        $(".uyeKayit").slideUp(2000);
        $(".uyeGirisi").slideDown(2000);
    });
});
$(document).ready(function(){
    $(".l1 a").click(function(){
        $(".sifreUnut").slideDown(2000);
        $(".uyeGirisi").slideUp(2000);
    });
});
$(document).ready(function(){
    $(".sifreUnutKapat").click(function(){
        $(".sifreUnut").slideUp(2000);
        $(".uyeGirisi").slideDown(2000);
    });
});



//Profil sayfası aside işlemleri

$(document).ready(function(){
    $(".larrowjs").click(function(){
        $(".profileAside").hide();
                
        if($(window).width() > 975){
            $('.col-lg-9').removeClass('col-lg-9').addClass('col-lg-12');
        }
    });
});
$(document).ready(function(){
    $(".toggleAside a").click(function(){
        $(".profileAside").show();
                
        if($(window).width() > 975){
            $('.col-lg-12').removeClass('col-lg-12').addClass('col-lg-9');
        }
    });
});

/* CK metin editörleri */
CKEDITOR.replace('urun_editor');

/* Resim seçerken resim adını yazdırmak için */
$(document).ready(function () {
    var fileName
    $('input[type="file"]').change(function (e) {
        fileName = e.target.files[0].name;
        $('#resim_name').text(fileName);
        $('#resim_hata').hide();
    });
    $('#btn_kaydet').click(function (e) {
        if (fileName == null) {
            $('#resim_hata').show();
            $('#resim_hata').text("Lütfen bir ürün resmi seçiniz!");
        }
    });
});

