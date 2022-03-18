
$(document).ready(function () {

    $('.register-info-box').fadeIn();
    $('.login-info-box').fadeOut();

    $('.white-panel').removeClass('right-log');

    $('.login-show').addClass('show-log-panel');
    $('.register-show').removeClass('show-log-panel');

    function fnUNAMEKeyUp() {
        $("#UNAMEHidden").val($("#txtUNAME").val());
    }
 

});