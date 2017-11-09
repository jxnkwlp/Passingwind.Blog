// Write your Javascript code.
'use strict';

$(function () {
    $(document).pjax('a[data-pjax]', '.content-area');

    $('#togglemenu').click(function (e) {
        $('#main-navigation').toggleClass('open');
        e.preventDefault();
    });
});

