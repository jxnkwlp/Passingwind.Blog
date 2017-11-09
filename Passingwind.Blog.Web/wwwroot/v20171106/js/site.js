// Write your Javascript code.
$(function () {
    $('#togglemenu').click(function (e) {
        $('#main-navigation').toggleClass('open');
        e.preventDefault();
    });
})
