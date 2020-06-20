/* Sidebar mobil cihazlarda açmak için toggle yöntemi */
$(document).ready(function(){
    $(".menu_toggle").click(function(){
        $(".sidebar").toggle(1000);
    });
});

/* Profile down açılımı */

$(document).ready(function(){
    $("#profile-down-link").click(function(){
        $(".profile-down").slideToggle();
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
