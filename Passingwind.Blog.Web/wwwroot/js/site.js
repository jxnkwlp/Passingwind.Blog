// Write your Javascript code.
$(function () {
    $(document).pjax('a[data-pjax]', '.content-area');

    $('#togglemenu').click(function (e) {
        $('#main-navigation').toggleClass('open');
        e.preventDefault();
    });
})
