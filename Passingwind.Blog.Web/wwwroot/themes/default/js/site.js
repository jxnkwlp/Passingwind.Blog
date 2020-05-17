// Write your Javascript code.
$(function () {
    // $(document).pjax('a[data-pjax]', '.content-area');

    $('#togglemenu').click(function (e) {
        $('#main-navigation').toggleClass('open');
        e.preventDefault();
    });
});

function toggleWidgetArchiveShow(ele) {
    var $this = $(ele).closest('li');
    $this.toggleClass('show');
    //if ($this.hasClass('show')) {
    //    $this.find('ul').hide();
    //} else {
    //    $this.find('ul').show();
    //}
}


window._alert = function (obj) {
    if (swal) {
        swal({
            title: obj.title,
            text: obj.text,
            icon: obj.icon || 'info',
        })
    } else {
        window.alert(obj.text);
    }
};
